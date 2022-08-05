using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Consts;

public class SelfManageButton : Button
{
    SelfManageButton_ManageOption manageOption;
    public void init(System.Action action)
    {
        onClick.RemoveAllListeners();
        onClick.AddListener(() => action());
    }

    System.Func<bool> canClick;
    public void SetButtonOption(System.Func<bool> clickOption)
    {
        manageOption = GetComponentInChildren<SelfManageButton_ManageOption>(); 
        canClick = clickOption;
    }

    private void Update() {
        if (manageOption == null || canClick == null) return;
            
        if (GameLoop.SkipFrame(frameOrder.refresh)) return;
        // NOTE: this is not the best way to do this, but it works for now
        bool clickAble = canClick();
        manageOption.SetButtonOption(clickAble);
        this.interactable = clickAble;
    }
    
}
