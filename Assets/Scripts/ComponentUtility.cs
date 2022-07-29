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
