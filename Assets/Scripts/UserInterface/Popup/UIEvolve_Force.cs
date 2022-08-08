using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Consts;

public class UIEvolve_Force : UIPanels
{
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private Button evolveButton;
    [SerializeField]
    private Text lblEvolve;

    public override void Show(List<textFactor> factors = null)
    {
        base.Show(factors);
    }

    public override void Init(ActionManager actionManager)
    {
        base.Init(actionManager);
        SetEvolveForceButton(actionManager);
    }

    void SetEvolveForceButton(ActionManager actionManager)
    {
        ComponentUtility.AddButtonAction(evolveButton, () => {
            actionManager.DoCreatureAction(CreatureActionType.evolve, int.Parse(lblEvolve.text));
            actionManager.DoConditionConsumeAction(ConditionCheckType.coin, GameLoop.needCoin);
            // actionManager.DoCreatureAction(CreatureActionType.evolve, bookIndex);
            exitButton.onClick.Invoke();
        });
    }
}
