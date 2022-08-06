using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Consts;
using UnityEngine.UI;

public class UIPlay : UIPanels
{

    [SerializeField]
    List<SelfManageButton> btnListPlay = new List<SelfManageButton>();


    public override void Init(ActionManager actionManager)
    {
        base.Init(actionManager);
        SetPlayButton(actionManager);
    }

    void SetPlayButton(ActionManager actionManager)
    {
        for (int i = 0; i < btnListPlay.Count; i++)
        {
            int needStamina = i * 10 + 5;
            int recoverValue = i * 15 + 20;
            btnListPlay[i].SetButtonOption(() =>
            {
                return (actionManager.CheckActionCondition(ConditionCheckType.stamina, needStamina) &&
                        actionManager.CheckActionCondition(ConditionCheckType.alive, 0));
            });

            ComponentUtility.SetButtonAction(btnListPlay[i], () =>
            {
                actionManager.DoStatusAction(StatusType.happiness, recoverValue);
                actionManager.DoStatusAction(StatusType.health, recoverValue);
                actionManager.DoConditionConsumeAction(ConditionCheckType.stamina, needStamina);
                actionManager.DoCreatureAction(CreatureState.Play, GameLoop.animationTime);
            });
        }
    }
}
