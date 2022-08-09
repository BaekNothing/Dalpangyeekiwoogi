using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Consts;

public class UIOption : UIPanels
{
    [SerializeField]
    SelfManageButton btnBuyCoffe;

    [SerializeField]
    SelfManageButton btnClearData;

    public override void Init(ActionManager actionManager)
    {
        base.Init(actionManager);
        SetBuyCoffeButton(actionManager);
    }
    
    void SetBuyCoffeButton(ActionManager actionManager)
    {
        ComponentUtility.SetButtonAction(
            btnBuyCoffe.GetComponent<Button>(),
            () => {
                ComponentUtility.Log("BuyCoffe");
                Application.OpenURL("https://www.buymeacoffee.com/baeknothing");
            });
        ComponentUtility.SetButtonAction(
            btnClearData.GetComponent<Button>(),
            () => {
                ComponentUtility.Log("ClearData");
                actionManager.DoUIPnlShowAction("clearplayerinfo");
                //actionManager.DoPlayerInfoAction(PlayerInfoActionType.initPlayerInfo, "");
            });
        
    }
}
