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

    public SingleStatus dirt;
    public SingleStatus happiness;
    public SingleStatus health;
    public SingleStatus hunger;

    public void ClearAllStat()
    {
        dirt.StatClear();
        happiness.StatClear();
        health.StatClear();
        hunger.StatClear();
    }

    public void InitAllStat()
    {
        dirt.StatInit();
        happiness.StatInit();
        health.StatInit();
        hunger.StatInit();
    }

    readonly static float valueMax = 100f;
    readonly static float valueMin = 0f;

    [Serializable]
    public struct SingleStatus
    {
        public string name;
        public float value;
        public int deadCount;
        public float deadTime;
        public float tick;
        public float subtraction;
        
        public void StatInit()
        {
            StatClear();
        }

        public void StatClear(){
            this.value = 100;
            this.deadCount = 0;
            this.deadTime = 0;
            this.tick = 900f;
            this.subtraction = 1;
        }

        public void StatCalculateTick(float correction = 1)
        {
            value -= subtraction * correction;
            if (value <= valueMin)
            { 
                deadTime += correction;
                value = valueMin;
            }
            if (deadTime >= 15) deadCount++;
        }        
    }
}
