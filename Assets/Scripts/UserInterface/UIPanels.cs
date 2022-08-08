using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanels : MonoBehaviour
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

    public UIManager uiManager { get; private set; }
    public int thisIndex;
    public void SetIndex(int index) => thisIndex = index;

    public void LinkManager(UIManager uiManager) =>
        this.uiManager = uiManager;

    public virtual void Init(ActionManager actionManager)
    {
        SetExit();
        // Do nothing
    }
    
    public virtual void Show(List<textFactor> factors = null)
    {
        SetTextFromFactors(factors);
    }

    void SetTextFromFactors(List<textFactor> factors)
    {
        if (factors == null || factors.Count == 0)
            return;
        foreach (textFactor factor in factors)
            ComponentUtility.SetText(
                ComponentUtility.FindT<Text>(this.transform, factor.targetName),
                factor.value);
    }

    void SetExit() =>
        ComponentUtility.SetButtonAction(
            ComponentUtility.FindT<Button>(this.transform, "exit"),
            () => {
                ComponentUtility.Log($"Exit : {this.name} {thisIndex}");
                uiManager.HidePanel(thisIndex);
            });

    public void Hide() =>
        uiManager.HidePanel(thisIndex);
}
