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
    [SerializeField]
    SelfManageButton btnTalk;
    [SerializeField]
    Text lblName;

   
    public override void Init(ActionManager actionManager)
    {
        base.Init(actionManager);
        SetTickAction(actionManager);
        SetDirtButton(actionManager);
        SetEvolveButton(actionManager);
        SetReviveButton(actionManager);
        SetTalkButton(actionManager);
    }

    void SetTickAction(ActionManager actionManager)
    {
        actionManager.RegistTickAction(() => {
            lblName.text =
                string.Join(" ", 
                actionManager.DoPlayerInfoAction(PlayerInfoActionType.getPlayerName, ""));
        });
    }

    void SetDirtButton(ActionManager actionManager)
    {

        float needStamina = 15;
        float recoverValue = 50;
        btnDirt.SetButtonOption(() =>
        {
            return (actionManager.CheckActionCondition(ConditionCheckType.stamina, needStamina) &&
                    actionManager.CheckActionCondition(ConditionCheckType.alive, 0));
        });

        ComponentUtility.SetButtonAction(btnDirt, () =>
        {
            actionManager.DoPropAction("broom");
            actionManager.DoStatusAction(StatusType.dirt, recoverValue);
            actionManager.DoConditionConsumeAction(ConditionCheckType.stamina, (int)needStamina);
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

    
    void SetTalkButton(ActionManager actionManager)
    {
        ComponentUtility.SetButtonAction(btnTalk, () =>
        {
            actionManager.DoPropAction("talk");
            actionManager.DoUIPnlShowAction("talk", new List<UIPanels.textFactor>{
               new UIPanels.textFactor(
                    "title",
                    GetRandomTalk(actionManager)
               )
            });
        });
    }

    List<string> talkList = new List<string>{
        "아무말",
        "아무말1",
        "아무말2",
        "축축한 곳이 좋아",
        "맛있는게 먹고싶어",
        "졸려"
    };

    string GetRandomTalk(ActionManager actionManager){
        
        if(!actionManager.CheckActionCondition(ConditionCheckType.dirt, 10))
            return "방이 너무 더러워 ㅠㅠ";
        else 
            return talkList[Random.Range(0, talkList.Count)];
    }
}
