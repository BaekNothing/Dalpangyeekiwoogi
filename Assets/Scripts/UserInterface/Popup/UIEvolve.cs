using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Consts;

public class UIEvolve : UIPanels
{
    [SerializeField]
    private Button evolveButton;

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
        ComponentUtility.AddButtonAction(evolveButton, () => {
            actionManager.DoCreatureAction(CreatureState.evolve, -1);
            actionManager.DoConditionAddAction(ConditionCheckType.coin, 10);
        });
    }
}
