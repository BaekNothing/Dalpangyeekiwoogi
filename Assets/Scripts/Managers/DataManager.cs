using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using Spine;
using Spine.Unity;
using Consts;


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

        RegistTickAction();
        RegistStatusAction();
        RegistQuitAction();

        actionManager.initFlag[nameof(DataManager)] = true;
    }
    

    void CalculateDealiedStat()
    {
        //Calculate stat while logging off
    }

    // ****** Tick Action *******

    void RegistTickAction()
    {
        actionManager.RegistTickAction(Action_CalcualteStat);
        actionManager.RegistTickAction(Action_CalculateStamina);
    }

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

    // ******* Stat Action *******
   
    void RegistStatusAction()
    {
        actionManager.RegistStatusAction(RecoverStatus);       
    }

    void RecoverStatus(StatusType type, float value)
    {
        SnailStat.AddStatusValue(type, value);
    }

    // ******* Quit Action *******

    void RegistQuitAction()
    {
        actionManager.RegistQuitAction(PlayerInfo.SetLastLoginTime);
    }
}