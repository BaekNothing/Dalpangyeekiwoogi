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

        ComponentUtility.LinkBtnPnl("option", btnDict, uiPanels, btnList);
        ComponentUtility.LinkBtnPnl("credits", btnDict, uiPanels, btnList);
        ComponentUtility.LinkBtnPnl("developplan", btnDict, uiPanels, btnList);
    }

    public void RegistPanel(UIPanels uiPanel)
    {
        uiPanels.Add(uiPanel);
        uiPanel.LinkManager(this);
        uiPanel.SetIndex(uiPanels.IndexOf(uiPanel));
    }
    
    [SerializeField]
    Stack<int> indexStack = new Stack<int>();

    public void ShowPanel(int index)
    {
        if (index < 0 || index >= uiPanels.Count)
            return;
        // foreach (UIPanels panel in uiPanels)
        //     panel.gameObject.SetActive(false);
        foreach (UIPanels pnl in ComponentUtility.FindAllT<UIPanels>(uiPanels[index].transform))
            pnl.gameObject.SetActive(false);
        uiPanels[index].gameObject.SetActive(true);
        uiPanels[index].Init();
        indexStack.Push(index);
    }

    public void ShowPanel(int index, List<UIPanels.textFactor> factor)
    {
        if (index < 0 || index >= uiPanels.Count)
            return;
        // foreach (UIPanels panel in uiPanels)
        //     panel.gameObject.SetActive(false);
        foreach (UIPanels pnl in ComponentUtility.FindAllT<UIPanels>(uiPanels[index].transform))
            pnl.gameObject.SetActive(false);
        uiPanels[index].gameObject.SetActive(true);
        uiPanels[index].Init(factor);
        indexStack.Push(index);
    }

    public void HidePanel(int index){
        if (index < 0 || index >= uiPanels.Count)
            return;
        if (index != indexStack.Peek())
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