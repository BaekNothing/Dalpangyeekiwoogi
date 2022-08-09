using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOption : UIPanels
{
    [SerializeField]
    SelfManageButton btnBuyCoffe;

    public override void Init(ActionManager actionManager)
    {
        base.Init(actionManager);
        SetBuyCoffeButton();
    }
    
    void SetBuyCoffeButton()
    {
        ComponentUtility.SetButtonAction(
            btnBuyCoffe.GetComponent<Button>(),
            () => {
                ComponentUtility.Log("BuyCoffe");
                Application.OpenURL("https://www.buymeacoffee.com/baeknothing");
            });
    }
}
