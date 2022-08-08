using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Consts;
using UnityEngine.UI;

public class UIPlay : UIPanels
{
    [SerializeField]
    SelfManageButton btnExercise;
    [SerializeField]
    SelfManageButton btnTv;
    [SerializeField]
    SelfManageButton btnRead;

    public override void Init(ActionManager actionManager)
    {
        base.Init(actionManager);
        SetExerciseButton(actionManager);
        SetTvButton(actionManager);
        SetReadButton(actionManager);
    }

    void SetExerciseButton(ActionManager actionManager)
    {
        float needStamina = 10f;
        float recoverHappiness = 20f;
        float recoverHealth = 40f;


        btnExercise.SetButtonOption(() =>
        {
            return (actionManager.CheckActionCondition(ConditionCheckType.stamina, needStamina) &&
                    actionManager.CheckActionCondition(ConditionCheckType.alive, 0));
        });
        ComponentUtility.SetButtonAction(btnExercise, () =>
        {
            actionManager.DoPropAction("exercise");
            actionManager.DoStatusAction(StatusType.happiness, recoverHappiness);
            actionManager.DoStatusAction(StatusType.health, recoverHealth);
            actionManager.DoConditionConsumeAction(ConditionCheckType.stamina, (int)needStamina);
            actionManager.DoCreatureAction(CreatureActionType.Eat, GameLoop.animationTime);
        });
    }

    void SetTvButton(ActionManager actionManager)
    {
        float needStamina = 10f;
        float recoverHappiness = 20f;
        float recoverHealth = 10f;

        btnTv.SetButtonOption(() =>
        {
            return (actionManager.CheckActionCondition(ConditionCheckType.stamina, needStamina) &&
                    actionManager.CheckActionCondition(ConditionCheckType.alive, 0));
        });
        ComponentUtility.SetButtonAction(btnTv, () =>
        {
            actionManager.DoPropAction("tv");
            actionManager.DoStatusAction(StatusType.happiness, recoverHappiness);
            actionManager.DoStatusAction(StatusType.health, recoverHealth);
            actionManager.DoConditionConsumeAction(ConditionCheckType.stamina, (int)needStamina);
            actionManager.DoCreatureAction(CreatureActionType.Eat, GameLoop.animationTime);
        });
    }

    void SetReadButton(ActionManager actionManager)
    {
        float needStamina = 10f;
        float recoverHappiness = 40f;
        float recoverHealth = 20f;

        btnRead.SetButtonOption(() =>
        {
            return (actionManager.CheckActionCondition(ConditionCheckType.stamina, needStamina) &&
                    actionManager.CheckActionCondition(ConditionCheckType.alive, 0));
        });
        ComponentUtility.SetButtonAction(btnRead, () =>
        {
            actionManager.DoPropAction("read");
            actionManager.DoStatusAction(StatusType.happiness, recoverHappiness);
            actionManager.DoStatusAction(StatusType.health, recoverHealth);
            actionManager.DoConditionConsumeAction(ConditionCheckType.stamina, (int)needStamina);
            actionManager.DoCreatureAction(CreatureActionType.Eat, GameLoop.animationTime);
        });
    }
}
