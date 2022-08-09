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
        
    public float GetStatusValue(StatusType type)
    {
        switch (type)
        {
            case StatusType.dirt:
                return dirt.value;
            case StatusType.happiness:
                return happiness.value;
            case StatusType.health:
                return health.value;
            case StatusType.hunger:
                return hunger.value;
            default:
                return 0;
        }
    }

    public float GetStatusDeadTime(StatusType type)
    {    
        switch (type)
        {
            case StatusType.dirt:
                return dirt.deadTime;
            case StatusType.happiness:
                return happiness.deadTime;
            case StatusType.health:
                return health.deadTime;
            case StatusType.hunger:
                return hunger.deadTime;
            default:
                return 0;
        }
    }

    public int GetDeadCount(StatusType type)
    {
        switch (type)
        {
            case StatusType.dirt:
                return dirt.deadCount;
            case StatusType.happiness:
                return happiness.deadCount;
            case StatusType.health:
                return health.deadCount;
            case StatusType.hunger:
                return hunger.deadCount;
            default:
                return 0;
        }
    }

    public void AddStatusValue(StatusType type, float value)
    {
        switch (type)
        {
            case StatusType.dirt:
                dirt.AddValue(value);
                break;
            case StatusType.happiness:
                happiness.AddValue(value);
                break;
            case StatusType.health:
                health.AddValue(value);
                break;
            case StatusType.hunger:
                hunger.AddValue(value);
                break;
            default:
                break;
        }
    }

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

    public bool CheckDead(float deadLimit)
    {
        if (dirt.deadCount > deadLimit || happiness.deadCount > deadLimit ||
            health.deadCount > deadLimit || hunger.deadCount > deadLimit)
            return true;
        return false;
    }

    public void InitAllStat(float tick, float subtraction)
    {
        dirt.StatInit(tick, subtraction);
        happiness.StatInit(tick, subtraction);
        health.StatInit(tick, subtraction);
        hunger.StatInit(tick, subtraction);
    }

    public Dictionary<string, float> GetAllStat()
    {
        Dictionary<string, float> stat = new Dictionary<string, float>();
        stat.Add("더러워졌어요", dirt.value);
        stat.Add("슬퍼졌어요", happiness.value);
        stat.Add("아파졌어요", health.value);
        stat.Add("배고파졌어요", hunger.value);
        return stat;
    }

    readonly static float valueMax = 130f;
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
        
        public void AddValue(float inputValue)
        {
            this.value += inputValue;
            if(this.value > valueMax)
                this.value = valueMax;
            else if(this.value < valueMin)
                this.value = valueMin;
        }

        public void StatInit(float tick, float subtraction){
            this.value = 100;
            this.deadCount = 0;
            this.deadTime = 0;
            this.tick = tick;
            this.subtraction = subtraction;
        }

        public void StatClear(){
            this.value = 100;
            this.deadCount = 0;
            this.deadTime = 0;
        }

        public void StatCalculateTick(float correction = 1)
        {
            this.value -= subtraction * correction;
            if (this.value <= valueMin)
            { 
                deadTime += correction;
                this.value = valueMin;
            }
            if (deadTime >= 15) 
            {
                deadCount++;
                deadTime = 0;
            }
        }        
    }
}
