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

    public void SetState(ManageOptionState state)
    {
        foreach(Transform child in GetComponentsInChildren<Transform>())
            if(child != this.transform)
                child.gameObject.SetActive(
                    child.name.ToLower().Contains(state.ToString().ToLower()));
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
