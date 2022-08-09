using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Consts;
using UnityEngine.UI;

public class UIBook : UIPanels
{

    [SerializeField]
    List<SelfManageButton> btnBook = new List<SelfManageButton>();
    
    public override void Init(ActionManager actionManager)
    {
        base.Init(actionManager);
        SetBtnBookList();
        SetBookButton(actionManager);
    }

    public void SetBtnBookList(){
        btnBook.Clear();
        ScrollRect scroll = ComponentUtility.FindT<ScrollRect>(this.transform, "scroll");
        if (scroll)
            for (int i = 0; i < scroll.content.childCount; i++)
                btnBook.Add(ComponentUtility.FindT<SelfManageButton>(scroll.content.GetChild(i), "button"));
    }

    void SetBookButton(ActionManager actionManager)
    {
        for (int i = 0; i < btnBook.Count; i++)
        {
            int index = i;
            btnBook[i].SetButtonOption(() =>
            {
                return (
                    actionManager.CheckActionCondition(ConditionCheckType.creatureList, index) &&
                    actionManager.CheckActionCondition(ConditionCheckType.coin, GameLoop.needCoin)
                );
            });

            int bookIndex = i;
            ComponentUtility.SetButtonAction(btnBook[i], () =>
            {
                // actionManager.DoConditionConsumeAction(ConditionCheckType.coin, needCoin);
                // actionManager.DoCreatureAction(CreatureActionType.evolve, bookIndex);
                actionManager.DoUIPnlShowAction("evolve_force", new List<UIPanels.textFactor>
                {     
                    new UIPanels.textFactor(
                        "title",
                        "xxx로 달팽이를 진화시켜볼까요?"
                    ),
                    
                    new UIPanels.textFactor(
                        "index",
                        index.ToString()
                    )
                });
            });
        }
    }
}
