using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Consts;
using UnityEngine.UI;

public class UIFood : UIPanels
{
    [SerializeField]
    SelfManageButton btnFood;
    [SerializeField]
    SelfManageButton btnMeat;
    [SerializeField]
    SelfManageButton btnSnak;

    public override void Init(ActionManager actionManager)
    {
        base.Init(actionManager);
        SetFoodButton(actionManager);
        SetMeatButton(actionManager);
        SetSnakButton(actionManager);
    }


    void SetFoodButton(ActionManager actionManager)
    {
        float needStamina = 10f;
        float recoverHunger = 20f;


        btnFood.SetButtonOption(() =>
        {
            return (actionManager.CheckActionCondition(ConditionCheckType.stamina, needStamina) &&
                    actionManager.CheckActionCondition(ConditionCheckType.alive, 0));
        });
        ComponentUtility.SetButtonAction(btnFood, () =>
        {
            actionManager.DoPropAction("food");
            actionManager.DoStatusAction(StatusType.hunger, recoverHunger);
            actionManager.DoConditionConsumeAction(ConditionCheckType.stamina, (int)needStamina);
            actionManager.DoCreatureAction(CreatureActionType.Eat, GameLoop.animationTime);
        });
    }

    void SetSnakButton(ActionManager actionManager)
    {
        float needStamina = 10f;
        float recoverHunger = 20f;


        btnSnak.SetButtonOption(() =>
        {
            return (actionManager.CheckActionCondition(ConditionCheckType.stamina, needStamina) &&
                    actionManager.CheckActionCondition(ConditionCheckType.alive, 0));
        });
        ComponentUtility.SetButtonAction(btnSnak, () =>
        {
            actionManager.DoPropAction("snak");
            actionManager.DoStatusAction(StatusType.hunger, recoverHunger);
            actionManager.DoConditionConsumeAction(ConditionCheckType.stamina, (int)needStamina);
            actionManager.DoCreatureAction(CreatureActionType.Eat, GameLoop.animationTime);
        });
    }

    void SetMeatButton(ActionManager actionManager)
    {
        float needStamina = 10f;
        float recoverHunger = 20f;


        btnMeat.SetButtonOption(() =>
        {
            return (actionManager.CheckActionCondition(ConditionCheckType.stamina, needStamina) &&
                    actionManager.CheckActionCondition(ConditionCheckType.alive, 0));
        });
        ComponentUtility.SetButtonAction(btnMeat, () =>
        {
            actionManager.DoPropAction("meat");
            actionManager.DoStatusAction(StatusType.hunger, recoverHunger);
            actionManager.DoConditionConsumeAction(ConditionCheckType.stamina, (int)needStamina);
            actionManager.DoCreatureAction(CreatureActionType.Eat, GameLoop.animationTime);
        });
    }

}
