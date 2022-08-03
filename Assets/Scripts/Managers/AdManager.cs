using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using Unity.Notifications.Android;
using System;

public class AdManager : MonoBehaviour
{
    Status status;

    public void _ShowRewardedAd()
    {
        float LastTime = 0f;
        float OnTime = 0f;
        CalculateUtility.TimeToNumber(PlayerPrefs.GetString("ADTime"), ref LastTime);
        CalculateUtility.TimeToNumber(System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm"), ref OnTime);

        if (OnTime - LastTime > 30) 
        {
            PlayerPrefs.SetString("ADTime", System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm"));
            
            float tempStamina = PlayerPrefs.GetFloat("Stamina");
            status?.StatusIncrease(100f, ref tempStamina);
            PlayerPrefs.SetFloat("Stamina", tempStamina);
        }
    }
}