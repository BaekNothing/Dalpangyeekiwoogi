using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using Unity.Notifications.Android;
using GoogleMobileAds.Api;
using System;

[CreateAssetMenu(fileName = "snailStatusObject", menuName = "SCObjects/SnailStatusObject", order = 1)]
public class SnailStatusObject : ScriptableObject 
{
    [SerializeField]
    public SingleStatus status;

    [Serializable]
    public struct SingleStatus
    {
        public string name;
        public float value;
        public int deadCount;

        public float tick;
        public float subtraction;
        
        public void StatInit()
        {
            this.value = PlayerPrefs.GetFloat($"status_{name}");
            this.deadCount = PlayerPrefs.GetInt($"{name}Stack");

            this.tick = 900f;
            this.subtraction = -1;
        }

        public void StatSave()
        {
            PlayerPrefs.SetFloat($"status_{name}", value);
            PlayerPrefs.SetInt($"{name}Stack", deadCount);
        }

        public void StatClear(){
            value = 0f;
            deadCount = 0;
        }

        public void StatCalculateTick(float correction = 1)
        {
            value += subtraction * correction;
            if (value <= 0)
                deadCount++;
        }        
    }
}
