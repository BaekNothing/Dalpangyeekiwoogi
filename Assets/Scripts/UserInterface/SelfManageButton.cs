using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelfManageButton : Button
{
    public Sprite enable = null;
    public Sprite disable = null;

    public void init(System.Action action)
    {
        onClick.RemoveAllListeners();
        onClick.AddListener(() => action());
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.image.sprite = enable;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.image.sprite = disable;
    }
}
