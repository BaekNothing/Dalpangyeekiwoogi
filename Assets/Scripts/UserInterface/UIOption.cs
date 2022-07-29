using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOption : UIPanels
{
    [SerializeField]
    List<SelfManageButton> btnList = null;
    public override void Init()
    {
        base.Init();
        btnList = ComponentUtility.FindAllT<SelfManageButton>(transform);
    }
    
}
