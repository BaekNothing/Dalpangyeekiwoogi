using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Consts;

public class ActionManager : MonoBehaviour
{
    public Dictionary<string, bool> initFlag = 
        new Dictionary<string, bool>{
            {nameof(UIManager), false},
            {nameof(DataManager), false},
            {nameof(CreatureManager), false},
            {nameof(ObjectManager), false},
        };
    
    bool AllClassReady = false;

    // ******* GAME LOOP ACTION *******

    [SerializeField]
    List<System.Action> initActionList = new List<System.Action>();
    public void RegistInitAction(System.Action action)
    {
        if (action != null && !initActionList.Contains(action))
            initActionList.Add(action);
    }
        

    [SerializeField]
    List<System.Action> tickActionList = new List<System.Action>();
    public void RegistTickAction (System.Action action) 
    {
        if (action != null && !tickActionList.Contains(action))
            tickActionList.Add(action);
    }

    Dictionary<KeyCode, List<System.Action>> keyActionDict = new Dictionary<KeyCode, List<System.Action>>();
    public void RegistKeyAction(KeyCode key, System.Action action)
    {
        if(!keyActionDict.ContainsKey(key))
            keyActionDict.Add(key, new List<System.Action>());
        if(action != null && !keyActionDict[key].Contains(action))
            keyActionDict[key].Add(action);
    }
    public void DoKeyAction(KeyCode key)
    {
        if(keyActionDict.ContainsKey(key))
        {
            foreach(var action in keyActionDict[key])
            {
                action();
            }
        }
    }


    List<System.Action> quitActionList = new List<System.Action>();
    public void RegistQuitAction(System.Action action) => quitActionList.Add(action);

    void Update()
    {
        if(!AllClassReady)
        {
            // Wait for all class ready
            if(AllClassReady = CheckClassReady())
                Init();
        }
        else// AllClassReady == true
        {
            if(GameLoop.SkipFrame(frameOrder.refresh)) return;
            // Central Game Loop
            foreach (System.Action action in tickActionList)
                action();
            foreach (KeyCode key in keyActionDict.Keys)
                if(Input.GetKeyDown(key))
                    foreach (System.Action action in keyActionDict[key])
                        action();
        }
    }

    void Init(){
        Application.targetFrameRate = 60;
        foreach(var action in initActionList)
            action();
        ComponentUtility.Log("AllClassReady");
    }

    bool CheckClassReady(){
        foreach(var flag in initFlag.Values)
            if(!flag)
                return false;
        return true;
    }

    private void OnApplicationQuit() {
        foreach (System.Action action in quitActionList)
            action();    
    }
    
    //****** ActiveAble Action (ex.Button) *******

    List<System.Action<string, List<UIPanels.textFactor>>> UIPnlShowActionList = new List<System.Action<string, List<UIPanels.textFactor>>>();
    public void RegistUIPnlShowAction(System.Action<string, List<UIPanels.textFactor>> action)
    {
        if (action != null && !UIPnlShowActionList.Contains(action))
            UIPnlShowActionList.Add(action);
    }
    public void DoUIPnlShowAction(string name, List<UIPanels.textFactor> factor = null)
    {
        foreach (System.Action<string, List<UIPanels.textFactor>> action in UIPnlShowActionList)
            action(name, factor);
    }

    List<System.Action<int>> evolveActionList = new List<System.Action<int>>();
    public void RegistEvolveAction(System.Action<int> action){
        if(action != null && !evolveActionList.Contains(action))
            evolveActionList.Add(action);
    } 
    public void DoEvolve(int index)
    {
        foreach (System.Action<int> action in evolveActionList)
            action(index);
    }

    List<System.Action<CreatureActionType, int>> creatureActionList = new List<System.Action<CreatureActionType, int>>();
    public void RegistCreatureAction(System.Action<CreatureActionType, int> action)
    {
        if(creatureActionList == null) creatureActionList = new List<System.Action<CreatureActionType, int>>();
        if(!creatureActionList.Contains(action))
            creatureActionList.Add(action);
    }
    public void DoCreatureAction(CreatureActionType actionType, int value = 0)
    {
        ComponentUtility.Log($"DoCreatureAction {actionType} {value}");
        foreach(var action in creatureActionList)
            action(actionType, value);
    }

    List<System.Action<StatusType, float>> statusActionDict = new List<System.Action<StatusType, float>>();
    public void RegistStatusAction(System.Action<StatusType, float> action)
    {
        if(statusActionDict == null) statusActionDict = new List<System.Action<StatusType, float>>();
        if(!statusActionDict.Contains(action))
            statusActionDict.Add(action);
    }

    public void DoStatusAction(StatusType statusType, float value)
    {
        foreach(var action in statusActionDict)
            action(statusType, value);
    }

    Dictionary<PlayerInfoActionType, List<System.Func<string, string>>> playerInfoActionDict = new Dictionary<PlayerInfoActionType, List<System.Func<string, string>>>();
    public void RegistPlayerInfoAction(PlayerInfoActionType actionType, System.Func<string, string> action)
    {
        if(!playerInfoActionDict.ContainsKey(actionType))
            playerInfoActionDict.Add(actionType, new List<System.Func<string, string>>());
        if(!playerInfoActionDict[actionType].Contains(action))
            playerInfoActionDict[actionType].Add(action);
    }
    public List<string> DoPlayerInfoAction(PlayerInfoActionType actionType, string value)
    {
        List<string> result = new List<string>();
        if(playerInfoActionDict.ContainsKey(actionType))
            foreach(var action in playerInfoActionDict[actionType])
                result.Add(action(value));
        return result;
    }


    // ****** List Condition of Actionable *******
    Dictionary<ConditionCheckType, List<System.Func<float, bool>>> conditionCheckDict = new Dictionary<ConditionCheckType, List<System.Func<float, bool>>>();
    
    public void RegistConditionalAction(ConditionCheckType actionCheckType, System.Func<float, bool> action)
    {
        if(!conditionCheckDict.ContainsKey(actionCheckType))
            conditionCheckDict.Add(actionCheckType, new List<System.Func<float, bool>>());
        if(!conditionCheckDict[actionCheckType].Contains(action))
            conditionCheckDict[actionCheckType].Add(action);
    }

    public bool CheckActionCondition(ConditionCheckType conditionType, float value)
    {
        foreach(var action in conditionCheckDict[conditionType])
            if(!action(value)) // if any action return false, return false
                return false;
        return true;
    }

    Dictionary<ConditionCheckType, List<System.Action<ConditionCheckType, int>>> conditionConsumeActionDict = new Dictionary<ConditionCheckType, List<System.Action<ConditionCheckType, int>>>();
    public void RegistConditionConsumeAction(ConditionCheckType conditionCheckType, System.Action<ConditionCheckType, int> action)
    {
        if(!conditionConsumeActionDict.ContainsKey(conditionCheckType))
            conditionConsumeActionDict.Add(conditionCheckType, new List<System.Action<ConditionCheckType, int>>());
        if(!conditionConsumeActionDict[conditionCheckType].Contains(action))
            conditionConsumeActionDict[conditionCheckType].Add(action);
    }
    public void DoConditionConsumeAction(ConditionCheckType conditionCheckType, int value)
    {
        if(conditionConsumeActionDict.ContainsKey(conditionCheckType))
            foreach(var action in conditionConsumeActionDict[conditionCheckType])
                action(conditionCheckType, value);
    }

    Dictionary<ConditionCheckType, List<System.Action<ConditionCheckType, int>>> conditionAddActionDict = new Dictionary<ConditionCheckType, List<System.Action<ConditionCheckType, int>>>();
    public void RegistConditionAddAction(ConditionCheckType conditionCheckType, System.Action<ConditionCheckType, int> action)
    {
        if(!conditionAddActionDict.ContainsKey(conditionCheckType))
            conditionAddActionDict.Add(conditionCheckType, new List<System.Action<ConditionCheckType, int>>());
        if(!conditionAddActionDict[conditionCheckType].Contains(action))
            conditionAddActionDict[conditionCheckType].Add(action);
    }
    public void DoConditionAddAction(ConditionCheckType conditionCheckType, int value)
    {
        if(conditionAddActionDict.ContainsKey(conditionCheckType))
            foreach(var action in conditionAddActionDict[conditionCheckType])
                action(conditionCheckType, value);
    }

    List<System.Action<string>> propActionList = new List<System.Action<string>>();
    public void RegistPropAction(System.Action<string> action)
    {
        if(action != null && !propActionList.Contains(action))
            propActionList.Add(action);
    }
    public void DoPropAction(string name)
    {
        foreach(var action in propActionList)
            action(name);
    }
}