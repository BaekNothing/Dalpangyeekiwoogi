using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelfManageButton : Button
{
    public Sprite enable = null;
    public Sprite disable = null;

    public Image iconImage = null;

    public void init(System.Action action)
    {
        onClick.RemoveAllListeners();
        onClick.AddListener(() => action());
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetSprite(iconImage, enable);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        SetSprite(iconImage, disable);
    }

    protected void SetSprite(Image iconImage, Sprite sprite)
    {
        if(!iconImage) return;
        iconImage.sprite = sprite;
    }
}
