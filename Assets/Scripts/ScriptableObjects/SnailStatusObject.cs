using System.Collections.Generic;
using UnityEngine;
using System;
using Consts;

[CreateAssetMenu(fileName = "snailStatusObject", menuName = "SCObjects/SnailStatusObject", order = 1)]
public class SnailStatusObject : ScriptableObject 
{
    [SerializeField]
    SingleStatus dirt;
    [SerializeField]
    SingleStatus happiness;
    [SerializeField]
    SingleStatus health;
    [SerializeField]
    SingleStatus hunger;


    
    Dictionary<StatusType, SingleStatus> statusDict;
    Dictionary<StatusType, SingleStatus> GetStatusDict() =>
        statusDict ?? (statusDict = new Dictionary<StatusType, SingleStatus>()
        {
            {StatusType.dirt, dirt},
            {StatusType.happiness, happiness},
            {StatusType.health, health},
            {StatusType.hunger, hunger}
        });
        
    public float GetStatusValue(StatusType type)
        => GetStatusDict()[type].value;
    public float GetStatusDeadTime(StatusType type)
        => GetStatusDict()[type].deadTime;

    public void AddStatusValue(StatusType type, float value) 
        => GetStatusDict()[type].AddValue(value);

    public void CalculateTickAllStat(float value)
    {
        dirt.StatCalculateTick(value);
        happiness.StatCalculateTick(value);
        health.StatCalculateTick(value);
        hunger.StatCalculateTick(value);
    }

    public void ClearAllStat()
    {
        dirt.StatClear();
        happiness.StatClear();
        health.StatClear();
        hunger.StatClear();
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
        
        public void AddValue(float value)
        {
            this.value += value;
            if(this.value > valueMax)
                this.value = valueMax;
            else if(this.value < valueMin)
                this.value = valueMin;
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
