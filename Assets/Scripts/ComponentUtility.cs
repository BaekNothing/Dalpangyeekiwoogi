using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ComponentUtility
{
    public static void SetText(Text text, string textString)
    {
        if(!text) return;
        text.text = textString;
    }

    public static void SetButtonAction(Button btn, System.Action action)
    {
        if (!btn || action == null) return;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => action());
    }

    public static void LinkBtnPnl(
        string keyward,
        SerializableDictionary<SelfManageButton, UIPanels> btnDict,
        List<UIPanels> uiPanels,
        List<SelfManageButton> btnList)
    {
        KeyValuePair<SelfManageButton, UIPanels> pair;
        if((pair = ExtractBtnPnl(keyward, uiPanels, btnList)).Key != null && pair.Value != null)
            btnDict.Add(pair.Key, pair.Value);
    }

    static KeyValuePair<SelfManageButton, UIPanels> ExtractBtnPnl(
        string keyward, 
        List<UIPanels> uiPanels,
        List<SelfManageButton> btnList)
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
        btn?.init(() => UICentralUnit.ShowPanel(pnl.thisIndex));
        return new KeyValuePair<SelfManageButton, UIPanels>(btn, pnl);
    }

    public static List<T> FindAllT<T>(Transform parent) where T : class
    {
        List<T> result = new List<T>();
        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            if (child.childCount != 0)
               result.AddRange(FindAllT<T>(child));
            T component = child.GetComponent<T>();
            if (component != null)
                result.Add(component);
        }
        return result;
    }

    public static T FindT<T>(Transform parent, string keyward) where T : Component
    {
        T target = null;
        if (parent.name.ToLower().Contains(keyward.ToLower()))
            target = parent.GetComponent<T>();
        if (target == null)
            for (int i = 0; i < parent.childCount; i++)
            {
                target = FindT<T>(parent.GetChild(i), keyward);
                if (target != null)
                    break;
            }
        return target;
    }
}

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    // this class from [https://redforce01.tistory.com/243]
    [SerializeField]
    private List<TKey> keys = new List<TKey>();
    [SerializeField]
    private List<TValue> values = new List<TValue>();
    // save the dictionary to lists
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }
    // load dictionary from lists
    public void OnAfterDeserialize()
    {
        this.Clear();
        if (keys.Count != values.Count)
            throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));
        for (int i = 0; i < keys.Count; i++)
            this.Add(keys[i], values[i]);
    }
}