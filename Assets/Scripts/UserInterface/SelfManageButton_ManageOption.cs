using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SelfManageButton))]
public class SelfManageButton_ManageOption : MonoBehaviour
{
    ManageOptionState state = ManageOptionState.icon;

    public enum ManageOptionState
    {
        icon,
        cant,
        info
    }

    public ManageOptionState GetState() => state;

    public void SetButtonOption(bool clickAble)
    {
        if (clickAble)
            state = ManageOptionState.icon;
        else
            state = ManageOptionState.cant;
        SetState(state, clickAble);
    }

    void SetState(ManageOptionState state, bool clickAble)
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject child = this.transform.GetChild(i).gameObject;
            if(!clickAble) 
                child.SetActive(false);
            else 
            {
                if (child.name.Contains("_"))
                    child.SetActive(true);
                else
                {
                    bool isStatConnectedObject 
                        = child.name.ToLower().Contains(state.ToString().ToLower());
                    child.SetActive(isStatConnectedObject);
                }
            }
        }
        this.state = state;
    }

    public bool ReadyToClick()
    {
        return state == ManageOptionState.info;
    }

    public bool ReturnToIcon()
    {
        return state != ManageOptionState.icon;
    }
}
