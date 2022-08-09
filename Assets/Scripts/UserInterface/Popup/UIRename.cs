using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Consts;

public class UIRename : UIPanels
{
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private Button renameButton;
    [SerializeField]
    private InputField lblRename;

    public override void Show(List<textFactor> factors = null)
    {
        base.Show(factors);
    }

    public override void Init(ActionManager actionManager)
    {
        base.Init(actionManager);
        SetRenameButton(actionManager);
    }

    void SetRenameButton(ActionManager actionManager)
    {
        ComponentUtility.AddButtonAction(renameButton, () =>
        {
            if(CheckRenameString(lblRename.text) && int.Parse(string.Join("", 
                actionManager.DoPlayerInfoAction(PlayerInfoActionType.getCoin, ""))) >= GameLoop.needCoin)
            {
                ComponentUtility.Log(string.Join(" ",
                    actionManager.DoPlayerInfoAction(PlayerInfoActionType.setPlayerName, lblRename.text)
                ));
                actionManager.DoConditionConsumeAction(ConditionCheckType.coin, GameLoop.needCoin);
                actionManager.DoKeyAction(KeyCode.Escape);
            }
            else 
            {
                ComponentUtility.Log("Not enough coin");
                lblRename.text = "동전이 모자라거나, 이름이 적절하지 않아요!";
            }
        });
    }

    bool CheckRenameString(string name)
    {
        if (name.Length > 0 && name.Length < 14)
            return true;
        return false;
    }
}
