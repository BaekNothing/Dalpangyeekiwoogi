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

    //maybe Can handle list not dictionary
    Dictionary<CreatureState, List<System.Action<CreatureState>>> creatureActionDict = new Dictionary<CreatureState, List<System.Action<CreatureState>>>();
    public void RegistCreatureAction(CreatureState state, System.Action<CreatureState> action)
    {
        if(!creatureActionDict.ContainsKey(state))
            creatureActionDict.Add(state, new List<System.Action<CreatureState>>());
        creatureActionDict[state].Add(action);
    }
    public void DoCreatureAction(CreatureState state)
    {
        if(creatureActionDict.ContainsKey(state))
            foreach(var action in creatureActionDict[state])
                action(state);
    }
    public List<System.Action<CreatureState>> GetCreatureAction(CreatureState state)
    {
        if(creatureActionDict.ContainsKey(state))
            return creatureActionDict[state];
        return null;
    }

    Dictionary<StatusType, List<System.Action<StatusType, float>>> statusActionDict = new Dictionary<StatusType, List<System.Action<StatusType, float>>>();
    public void RegistStatusAction(StatusType statusType, System.Action<StatusType, float> action)
    {
        if(!statusActionDict.ContainsKey(statusType))
            statusActionDict.Add(statusType, new List<System.Action<StatusType, float>>());
        statusActionDict[statusType].Add(action);
    }

    public void DoStatusAction(StatusType statusType, float value)
    {
        if(statusActionDict.ContainsKey(statusType))
            foreach(var action in statusActionDict[statusType])
                action(statusType, value);
    }

    public List<System.Action<StatusType, float>> GetStatusAction(StatusType statusType)
    {
        if(statusActionDict.ContainsKey(statusType))
            return statusActionDict[statusType];
        return null;
    }

    [SerializeField]
    List<System.Action> initActionList = new List<System.Action>();
    public void RegistInitAction(System.Action action)
        => initActionList.Add(action);

    [SerializeField]
    List<System.Action> tickActionList = new List<System.Action>();
    public void RegistTickAction (System.Action action) => tickActionList.Add(action);

    public enum Condition { isBigger, isSmaller,isEqual }

    Dictionary<KeyCode, List<System.Action>> keyActionDict = new Dictionary<KeyCode, List<System.Action>>();
    public void RegistKeyAction(KeyCode key, System.Action action)
    {
        if(!keyActionDict.ContainsKey(key))
            keyActionDict.Add(key, new List<System.Action>());
        keyActionDict[key].Add(action);
    }

    List<System.Action> quitAction = new List<System.Action>();
    public void RegistQuitAction(System.Action action) => quitAction.Add(action);

    void Init(){
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

    private void OnApplicationQuit() {
        foreach (System.Action action in quitAction)
            action();    
    }
    
    // ****** Action *******

}
