using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    
    public override void OnPointerClick(PointerEventData eventData)
    {
        if(manageOption == null)
            base.OnPointerClick(eventData);
        else
        {
            // icon ->  canClick() -> cant -> icon
            //                     -> info -> icon
            if(canClick != null && !canClick())
            {
                if (manageOption.ReturnToIcon())
                    manageOption.SetState(SelfManageButton_ManageOption.ManageOptionState.icon);
                else
                    manageOption.SetState(SelfManageButton_ManageOption.ManageOptionState.cant);
            }
            else
            {
                if(manageOption.ReadyToClick())
                {
                    base.OnPointerClick(eventData);
                    manageOption.SetState(SelfManageButton_ManageOption.ManageOptionState.icon);
                }
                else
                    manageOption.SetState(SelfManageButton_ManageOption.ManageOptionState.info);
            }
        }
    }
}
