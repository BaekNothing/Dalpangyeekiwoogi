using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Consts;

public class UIStamina : UIPanels
{
    [SerializeField]
    Text usedStamina;
    
    public override void Init(ActionManager actionManager)
    {
        base.Init(actionManager);
    }
}
