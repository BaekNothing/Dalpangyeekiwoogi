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
        coin
    }

    public enum CreatureState 
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