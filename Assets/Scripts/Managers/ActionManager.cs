using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public Dictionary<string, bool> initFlag = 
        new Dictionary<string, bool>{
            {nameof(UIManager), false},
            {nameof(DataManager), false},
            {nameof(CreatureManager), false},
        };
    bool AllClassReady = false;

    Dictionary<CreatureManager.CreatureState, List<System.Action>> creatureActionDict = new Dictionary<CreatureManager.CreatureState, List<System.Action>>();
    public void RegistCreatureAction(CreatureManager.CreatureState state, System.Action action)
    {
        if(!creatureActionDict.ContainsKey(state))
            creatureActionDict.Add(state, new List<System.Action>());
        creatureActionDict[state].Add(action);
    }
    public void DoCreatureAction(CreatureManager.CreatureState state)
    {
        if(creatureActionDict.ContainsKey(state))
            foreach(var action in creatureActionDict[state])
                action();
    }
    public List<System.Action> GetCreatureAction(CreatureManager.CreatureState state)
    {
        if(creatureActionDict.ContainsKey(state))
            return creatureActionDict[state];
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
    public List<OptionalAction> optionalActionList = new List<OptionalAction>();
    public void RegistAction(OptionalAction action) => optionalActionList.Add(action);

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
            // Wait for all class ready
            if(AllClassReady = CheckClassReady())
                Init();
        else// AllClassReady == true
        {
            // Central Game Loop
            foreach (System.Action action in tickActionList)
                action();
            foreach (OptionalAction action in optionalActionList)
                DoOptionalAction(action);
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

    public struct OptionalAction
    {
        public string actionKey;
        public bool isDisposable;

        public SnailStatusObject.SingleStatus target;
        public Condition condition;
        public float value;
        public System.Action action;
        
        OptionalAction
        (string actionKey, 
            SnailStatusObject.SingleStatus target, 
            Condition condition, 
            float value, 
            System.Action action, 
            bool isDisposable = false)
        {
            this.actionKey = actionKey;
            this.target = target;
            this.condition = condition;
            this.value = value;
            this.action = action;
            this.isDisposable = isDisposable;
        }
    }
    
    void DoOptionalAction(OptionalAction inputAction)
    {
        //much more memory but more readable
        SnailStatusObject.SingleStatus target = inputAction.target;
        Condition condition = inputAction.condition;
        float value = inputAction.value;
        System.Action action = inputAction.action;

        if (condition == Condition.isBigger)
            if (target.value > value)
                action();
        else if (condition == Condition.isSmaller)
            if (target.value < value)
                action();
        else if (condition == Condition.isEqual)
            if (target.value == value)
                action();
            
        if (inputAction.isDisposable)
            optionalActionList.Remove(inputAction);
    }
}
