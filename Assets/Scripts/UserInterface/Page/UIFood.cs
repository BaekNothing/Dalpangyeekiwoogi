using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Consts;
using UnityEngine.UI;

public class UIFood : UIPanels
{
    [SerializeField]
    List<SelfManageButton> btnListFood = new List<SelfManageButton>();

    public override void Init(ActionManager actionManager)
    {
        base.Init(actionManager);
        SetFoodButton(actionManager);
    }


    void SetFoodButton(ActionManager actionManager)
    {
        for (int i = 0; i < btnListFood.Count; i++)
        {
            int needStamina = i * 10 + 5;
            int recoverValue = i * 15 + 20;
            btnListFood[i].SetButtonOption(() =>
            {
                return (actionManager.CheckActionCondition(ConditionCheckType.stamina, needStamina) &&
                        actionManager.CheckActionCondition(ConditionCheckType.alive, 0));
            });

            ComponentUtility.SetButtonAction(btnListFood[i], () =>
            {
                actionManager.DoStatusAction(StatusType.hunger, recoverValue);
                actionManager.DoConditionConsumeAction(ConditionCheckType.stamina, needStamina);
                actionManager.DoCreatureAction(CreatureState.Eat, GameLoop.animationTime);
            });
        }

    }

}
