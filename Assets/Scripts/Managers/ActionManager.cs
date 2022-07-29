using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    
    [SerializeField]
    List<System.Action> tickActionList = new List<System.Action>();
    public void RegistTickAction (System.Action action) => tickActionList.Add(action);

    public enum Condition { isBigger, isSmaller,isEqual }
    public List<OptionalAction> optionalActionList = new List<OptionalAction>();
    public void RegistAction(OptionalAction action) => optionalActionList.Add(action);


    void Update()
    {
        foreach (System.Action action in tickActionList)
            action();
        foreach (OptionalAction action in optionalActionList)
            DoOptionalAction(action);
        if (Input.GetKeyDown(KeyCode.Escape)) {
            UICentralUnit.HideAllPanel();
        }
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
