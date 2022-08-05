using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Consts;


[RequireComponent(typeof(Text))]
public class CurrencyText : MonoBehaviour
{
    [SerializeField]
    PlayerInfoObject playerInfo;

    [SerializeField]
    string currencyName;

    [SerializeField]
    object currencyValue;
    Text currency;
    

    void Awake()
    {
        currency = GetComponent<Text>();
        if (playerInfo == null)
            ComponentUtility.LogError("playerInfo is null");
        else
        {
            System.Type playerInfoClass = playerInfo.GetType();
            currencyValue = playerInfoClass?.GetField(currencyName)?.GetValue(playerInfo);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(GameLoop.SkipFrame(frameOrder.refresh)) return;
        currency.text = currencyValue?.ToString();
    }

    
}
