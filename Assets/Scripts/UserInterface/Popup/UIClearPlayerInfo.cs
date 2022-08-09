using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Consts;

public class UIClearPlayerInfo : UIPanels
{
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private Button clearButton;
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
        ComponentUtility.AddButtonAction(clearButton, () =>
        {
            actionManager.DoPlayerInfoAction(PlayerInfoActionType.initPlayerInfo, "0");
            actionManager.DoCreatureAction(CreatureActionType.evolve, 0);
            actionManager.DoKeyAction(KeyCode.Escape);
            exitButton.onClick.Invoke();
        });
    }
}
