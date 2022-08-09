using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Consts;


public class UITop : UIPanels
{
    [SerializeField]
    Text lblCoin;

    [SerializeField]
    Text lblStamina;

    [SerializeField]
    Text lblRamainStamina;

    public override void Init(ActionManager actionManager)
    {
        base.Init(actionManager);
        Action_TickAction(actionManager);
    }

    void Action_TickAction(ActionManager actionManager)
    {
        if (GameLoop.SkipFrame(frameOrder.refresh)) return;
        lblRamainStamina.text = MakeListStringToString(
            actionManager.DoPlayerInfoAction(PlayerInfoActionType.getStaminaRemainTime, ""));

        lblStamina.text = MakeListStringToString(
            actionManager.DoPlayerInfoAction(PlayerInfoActionType.getStamina, ""));

        lblCoin.text = MakeListStringToString(
            actionManager.DoPlayerInfoAction(PlayerInfoActionType.getCoin, ""));
    }

    string MakeListStringToString(List<string> listString){
        string result = "";
        for (int i = 0; i < listString.Count; i++)
            result += listString[i];
        return result;
    }
}
