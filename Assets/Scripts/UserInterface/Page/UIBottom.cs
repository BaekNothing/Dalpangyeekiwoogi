using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Consts;
using UnityEngine.UI;

public class UIBottom : UIPanels
{

    [SerializeField]
    SelfManageButton btnEvolve;
    [SerializeField]
    SelfManageButton btnDirt;
    [SerializeField]
    SelfManageButton btnRevive;
   
    public override void Init(ActionManager actionManager)
    {
        base.Init(actionManager);

        SetDirtButton(actionManager);
        SetEvolveButton(actionManager);
        SetReviveButton(actionManager);
    }

    void SetDirtButton(ActionManager actionManager)
    {

        int needStamina = 50;
        int recoverValue = 100;
        btnDirt.SetButtonOption(() =>
        {
            return (actionManager.CheckActionCondition(ConditionCheckType.stamina, needStamina) &&
                    actionManager.CheckActionCondition(ConditionCheckType.alive, 0));
        });

        ComponentUtility.SetButtonAction(btnDirt, () =>
        {

            actionManager.DoStatusAction(StatusType.dirt, recoverValue);
            actionManager.DoConditionConsumeAction(ConditionCheckType.stamina, needStamina);
            actionManager.DoCreatureAction(CreatureActionType.Clean, GameLoop.animationTime);
        });
    }

    void SetEvolveButton(ActionManager actionManager)
    {
        btnEvolve.SetButtonOption(() =>
        {
            return (actionManager.CheckActionCondition(ConditionCheckType.evolve, 0) &&
                    actionManager.CheckActionCondition(ConditionCheckType.alive, 0));
        });

        ComponentUtility.SetButtonAction(btnEvolve, () =>
        {
            actionManager.DoUIPnlShowAction("evolve", new List<UIPanels.textFactor>
            {

            });
            // actionManager.DoCreatureAction(CreatureActionType.evolve, -1);
            // actionManager.DoConditionAddAction(ConditionCheckType.coin, 10);
        });
    }

    void SetReviveButton(ActionManager actionManager)
    {
        btnRevive.SetButtonOption(() =>
        {
            return !(actionManager.CheckActionCondition(ConditionCheckType.alive, 0));
        });

        ComponentUtility.SetButtonAction(btnRevive, () =>
        {
            actionManager.DoUIPnlShowAction("revive", new List<UIPanels.textFactor>{
               
            });
            // actionManager.DoEvolve(0);
        });
    }
}
