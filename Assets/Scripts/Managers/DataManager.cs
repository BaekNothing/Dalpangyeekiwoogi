using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using Spine;
using Spine.Unity;


public class DataManager : MonoBehaviour
{
    ActionManager actionManager;

    enum playerPerfOption
    {
        IndexNumber,
    }
    [SerializeField]
    private PlayerInfoObject _playerInfo;
    public PlayerInfoObject PlayerInfo { get { return _playerInfo; } }

    [SerializeField]
    private SnailStatusObject _snailStat;
    public SnailStatusObject SnailStat { get { return _snailStat; } }
    
    [SerializeField]
    private CreatureDataObject _creature;
    public CreatureDataObject Creature { get { return _creature; } }

    void Awake()
    {
        Creature.LoadCreatureData();
        Creature.LoadSkeletonData();
        if(!PlayerInfo.isLoaded) 
            PlayerInfo.CheckLegacyPrefs(_creature);

        SnailStat.InitAllStat(900f, 1f);
        SnailStat.ClearAllStat();

        actionManager = this.GetComponent<ActionManager>();
        //Calculating data will be start later than init game    
        actionManager.RegistTickAction(Action_CalcualteStat);
        actionManager.RegistTickAction(Action_CalculateStamina);

        actionManager.RegistQuitAction(PlayerInfo.SetLastLoginTime);
        actionManager.initFlag[nameof(DataManager)] = true;
    }
    
    // ****** Stat Action *******

    enum frameOrder {
        stat = 0,
        stamina
    }

    bool SkipFrame(frameOrder order)
    {
        int frameCount = Time.frameCount;
        int frameRemainder = (int)(frameCount * 0.05);
        if (frameCount - frameRemainder * 20 != (int)order)
            return false;
        return true;
    }

    readonly float tickCorrection = 0.02f; //= 1 / 900 * 20
    public void Action_CalcualteStat()
    {
        // Action when frameCount % 20 == 0        
        if (SkipFrame(frameOrder.stat))
            return;
        SnailStat.CalculateTickAllStat(Time.deltaTime * tickCorrection);
    }

    public void Action_CalculateStamina()
    {
        // Action when frameCount % 20 == 0        
        if (SkipFrame(frameOrder.stamina))
            return;

        if (PlayerInfo.GetPassedStaminaTime() >= 15)
            PlayerInfo.RecoverStamina();
    }

}