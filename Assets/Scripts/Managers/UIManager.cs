using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

// OtherClass <==> UIManager <==> UIUtils <==> OtherUIPanels

public class UIManager : MonoBehaviour
{
    ActionManager actionManager;

    [SerializeField]
    Canvas mainCanvas;
    [SerializeField]
    List<UIPanels> uiPanels = null;
    [SerializeField]
    List<SelfManageButton> btnList = null;

    [SerializeField]
    SerializableDictionary<SelfManageButton, UIPanels> btnDict = new SerializableDictionary<SelfManageButton, UIPanels>();
    
    private void Awake()
    {
        mainCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
        actionManager = this.GetComponent<ActionManager>();
        actionManager.RegistKeyAction(KeyCode.Escape, HideAllPanel);

        foreach(UIPanels pnl in ComponentUtility.FindAllT<UIPanels>(mainCanvas.transform))
            RegistPanel(pnl);
        uiPanels.ForEach(x => x.LinkManager(this));
        btnList = ComponentUtility.FindAllT<SelfManageButton>(mainCanvas.transform);


        //******* Stage *********//
        ComponentUtility.LinkBtnPnl("talk", btnDict, uiPanels, btnList);

        //******* TopPanel *********//
        ComponentUtility.LinkBtnPnl("option", btnDict, uiPanels, btnList);
            ComponentUtility.LinkBtnPnl("credits", btnDict, uiPanels, btnList);
            ComponentUtility.LinkBtnPnl("developplan", btnDict, uiPanels, btnList);

        ComponentUtility.LinkBtnPnl("book", btnDict, uiPanels, btnList);
        ComponentUtility.LinkBtnPnl("stamina", btnDict, uiPanels, btnList);
        ComponentUtility.LinkBtnPnl("shop", btnDict, uiPanels, btnList);

        //******** Bottom *********//
        ComponentUtility.LinkBtnPnl("food", btnDict, uiPanels, btnList);
        ComponentUtility.LinkBtnPnl("play", btnDict, uiPanels, btnList);

        actionManager.initFlag[nameof(UIManager)] = true;
    }

    public void RegistPanel(UIPanels uiPanel)
    {
        uiPanels.Add(uiPanel);
        uiPanel.LinkManager(this);
        uiPanel.SetIndex(uiPanels.IndexOf(uiPanel));
    }
    
    [SerializeField]
    Stack<int> indexStack = new Stack<int>();

    public void ShowPanel(int index, List<UIPanels.textFactor> factor = null)
    {
        if (index < 0 || index >= uiPanels.Count)
            return;
        // foreach (UIPanels panel in uiPanels)
        //     panel.gameObject.SetActive(false);
        foreach (UIPanels pnl in ComponentUtility.FindAllT<UIPanels>(uiPanels[index].transform))
            HidePanel(pnl);
        uiPanels[index].gameObject.SetActive(true);
        uiPanels[index].Init(factor);
        indexStack.Push(index);
    }

    public void HidePanel(UIPanels pnl)
        => HidePanel(pnl.thisIndex);

    public void HidePanel(int index){
        if (index < 0 || index >= uiPanels.Count)
            return;
        if (indexStack.Count <= 0 || index != indexStack.Peek())
            return;
        uiPanels[index].gameObject.SetActive(false);
        indexStack.Pop();
    }

    public void HideAllPanel(){
        foreach (UIPanels panel in uiPanels)
            panel.gameObject.SetActive(false);
        indexStack.Clear();
    }
}