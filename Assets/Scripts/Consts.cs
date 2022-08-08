using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Consts
{
    public enum ConditionCheckType
    {
        stamina,
        alive,
        evolve,
        coin,
        creatureList
    }

    public enum CreatureActionType 
    {   
        Clean,
        Eat, 
        Play, 
        stand,
        dead,
        evolve
    }

    public enum StatusType
    {
        dirt,
        happiness,
        health,
        hunger
    }

    public enum frameOrder {
        stat = 0,
        stamina = 5,
        evolve = 10,
        dead = 15,
        refresh
    }

    static class GameLoop
    {
        static public readonly float tickCorrection = 0.005f; //= 1 / 900 * 40
        static public readonly int animationTime = 2;
        static public readonly float deadTimeLimit = 3f; // 1f => 1sec * 100 = 1.4Min
        static public readonly float deadLimit = 192f; // 192f => 960min(=16hour * 60min) / 5 
        static public readonly float evolveLimit = 8460f; // 8460 min = 2 day
        static public readonly float staminaLimitTime = 15f;

        static public bool SkipFrame(frameOrder order)
        {
            // frame division is 20
            int frameCount = Time.frameCount;
            int frameRemainder = (int)(frameCount * 0.05);
            if (frameCount - frameRemainder * 20 != (int)order)
                return false;
            return true;
        }
    }
}