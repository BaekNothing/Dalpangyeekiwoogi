using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRevive : UIPanels
{
    [SerializeField]
    private Button reviveButton;

    public override void Show(List<textFactor> factors = null)
    {
        base.Show(factors);
    }

    public override void Init(ActionManager actionManager)
    {
        base.Init(actionManager);
        SetReviveButton(actionManager);
    }

    void SetReviveButton(ActionManager actionManager)
    {
        ComponentUtility.AddButtonAction(reviveButton, ()=>actionManager.DoEvolve(0));
    }
}