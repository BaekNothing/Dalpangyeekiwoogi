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
    
    // ****** ActiveAble Action (ex.Button) *******

    List<System.Action<CreatureState>> creatureActionList = new List<System.Action<CreatureState>>();
    public void RegistCreatureAction(System.Action<CreatureState> action)
    {
        if(creatureActionList == null) creatureActionList = new List<System.Action<CreatureState>>();
        if(!creatureActionList.Contains(action))
            creatureActionList.Add(action);
    }
    public void DoCreatureAction(CreatureState state)
    {
        foreach(var action in creatureActionList)
            action(state);
    }
    public List<System.Action<CreatureState>> GetCreatureAction(CreatureState state)
    {
        return creatureActionList;
    }

    List<System.Action<StatusType, float, float>> statusActionDict = new List<System.Action<StatusType, float, float>>();
    public void RegistStatusAction(System.Action<StatusType, float, float> action)
    {
        if(statusActionDict == null) statusActionDict = new List<System.Action<StatusType, float, float>>();
        if(!statusActionDict.Contains(action))
            statusActionDict.Add(action);
    }

    public void DoStatusAction(StatusType statusType, float value, float stamina)
    {
        foreach(var action in statusActionDict)
            action(statusType, value, stamina);
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
}