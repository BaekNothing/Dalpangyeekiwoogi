using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Consts;
using UnityEngine.UI;

public class UIBook : UIPanels
{

    [SerializeField]
    List<SelfManageButton> btnBook = new List<SelfManageButton>();
    List<string> bookData = new List<string>();
    
    public override void Init(ActionManager actionManager)
    {
        base.Init(actionManager);
        SetBtnBookList(actionManager);
        SetBookButton(actionManager);
    }

    public void SetBtnBookList(ActionManager actionManager){
        btnBook.Clear();
        ScrollRect scroll = ComponentUtility.FindT<ScrollRect>(this.transform, "scroll");
        if (scroll)
            for (int i = 0; i < scroll.content.childCount; i++)
            {
                SelfManageButton btn = ComponentUtility.FindT<SelfManageButton>(scroll.content.GetChild(i), "button");
                if (btn)
                    btnBook.Add(btn);
                
                Text name = ComponentUtility.FindT<Text>(scroll.content.GetChild(i), "name");
                bookData.Add(MakeListStringToString(
                    actionManager.DoPlayerInfoAction(PlayerInfoActionType.getCreatureData, i.ToString())));
                if (name)
                    name.text = $"{bookData[i].Split('^')[0]}";
            } 
    }

    string MakeListStringToString(List<string> listString)
    {
        string result = "";
        for (int i = 0; i < listString.Count; i++)
            result += listString[i];
        return result;
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
                        $"{bookData[bookIndex].Split('^')[0]}로 달팽이를 진화시켜볼까요?"
                    ),

                    new UIPanels.textFactor(
                        "desc",
                        $"{bookData[bookIndex].Split('^')[1]}"
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
