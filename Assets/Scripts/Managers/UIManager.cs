using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

// OtherClass <==> UIManager <==> UIUtils <==> OtherUIPanels

public class UIManager : MonoBehaviour
{
    [SerializeField]
    Canvas mainCanvas;
    [SerializeField]
    List<UIPanels> uiPanels = null;
    [SerializeField]
    List<SelfManageButton> btnList = null;
    Dictionary<SelfManageButton, UIPanels> btnDict = new Dictionary<SelfManageButton, UIPanels>();

    private void Awake()
    {
        mainCanvas = GameObject.Find("MainCanvas").GetComponent<Canvas>();
        uiPanels = ComponentUtility.FindAllT<UIPanels>(mainCanvas.transform);
        uiPanels.ForEach(x => x.Regist());
        btnList = ComponentUtility.FindAllT<SelfManageButton>(mainCanvas.transform);
        LinkBtnPnl();
    }

    void LinkBtnPnl(){
        KeyValuePair<SelfManageButton, UIPanels> pair;
        if((pair = ExtractBtnPnl("option")).Key != null && pair.Value != null)
            btnDict.Add(pair.Key, pair.Value);
    }

    KeyValuePair<SelfManageButton, UIPanels> ExtractBtnPnl(string keyward)
    {
        UIPanels pnl = null;
        foreach (var item in uiPanels)
            if (item.name.ToLower().Contains(keyward.ToLower()))
            {
                pnl = item;
                break;
            }
        SelfManageButton btn = null;
        foreach (var item in btnList)
            if (item.name.ToLower().Contains(keyward.ToLower()))
            {
                btn = item;
                break;
            }
        btn.init(() => UICentralUnit.ShowPanel(pnl.thisIndex));
        return new KeyValuePair<SelfManageButton, UIPanels>(null, null);
    }
}

public static class UICentralUnit
{
    [SerializeField]
    static List<UIPanels> uiPanels = new List<UIPanels>();
    public static int RegistPanel(UIPanels panel)
    {
        uiPanels.Add(panel);
        return uiPanels.IndexOf(panel);
    }
    
    [SerializeField]
    static Stack<int> indexStack = new Stack<int>();

    static public void ShowPanel(int index)
    {
        if (index < 0 || index >= uiPanels.Count)
            return;
        foreach (UIPanels panel in uiPanels)
            panel.gameObject.SetActive(false);
        uiPanels[index].gameObject.SetActive(true);
        uiPanels[index].Init();
        indexStack.Push(index);
    }

    static public void ShowPanel(int index, List<UIPanels.textFactor> factor)
    {
        if (index < 0 || index >= uiPanels.Count)
            return;
        foreach (UIPanels panel in uiPanels)
            panel.gameObject.SetActive(false);
        uiPanels[index].gameObject.SetActive(true);
        uiPanels[index].Init(factor);
        indexStack.Push(index);
    }

    static public void HidePanel(int index){
        if (index < 0 || index >= uiPanels.Count)
            return;
        if (index != indexStack.Peek())
            return;
        uiPanels[index].gameObject.SetActive(false);
        indexStack.Pop();
    }

    static public void HideAllPanel(){
        foreach (UIPanels panel in uiPanels)
            panel.gameObject.SetActive(false);
        indexStack.Clear();
    }
}

public abstract class UIPanels : MonoBehaviour
{
    public struct textFactor
    {
        public string targetName;
        public string value;
        public textFactor(string targetName, string value)
        {
            this.targetName = targetName;
            this.value = value;
        }
    }

    public int thisIndex;

    public void Regist() =>
        UICentralUnit.RegistPanel(this);

    public virtual void Init(List<textFactor> factors)
    {
        SetTextFromFactors(factors);
        SetExit();
    }

    void SetTextFromFactors(List<textFactor> factors)
    {
        foreach(textFactor factor in factors)
            ComponentUtility.SetText(
                ComponentUtility.FindT<Text>(this.transform, factor.targetName),
                factor.value);
    }
        
    public virtual void Init() =>
        SetExit();

    void SetExit()=>
        ComponentUtility.SetButtonAction(
            ComponentUtility.FindT<Button>(this.transform, "exit"),
            () => UICentralUnit.HidePanel(thisIndex));

    public void Hide() =>
        UICentralUnit.HidePanel(thisIndex);
} 
