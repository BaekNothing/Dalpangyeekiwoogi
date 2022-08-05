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
        RegistConditionalAction();
        RegistConditionConsumeAction();
        RegistConditionAddAction();

        actionManager.initFlag[nameof(DataManager)] = true;
    }
    

    void CalculateDealiedStat()
    {
        double passedMin = PlayerInfo.GetPassedLoginTime();
        for (int i = 0; i < passedMin * 60; i++)
        {
            //decrease stat 0.02f per second
            SnailStat.CalculateTickAllStat(0.02f);
            Action_CheckEvolve();
            Action_CheckDead();
        }
        
    }

    // ****** Tick Action *******

    void RegistTickAction()
    {
        actionManager.RegistTickAction(Action_CalcualteStat);
        actionManager.RegistTickAction(Action_CalculateStamina);
        actionManager.RegistTickAction(Action_CheckDead);
        actionManager.RegistTickAction(Action_CheckEvolve);
    }

    readonly float tickCorrection = 0.02f; //= 1 / 900 * 20
    void Action_CalcualteStat()
    {
        // Action when frameCount % 20 == 0        
        if (GameLoop.SkipFrame(frameOrder.stat))
            return;
        SnailStat.CalculateTickAllStat(Time.deltaTime * tickCorrection);
    }

    void Action_CalculateStamina()
    {
        // Action when frameCount % 20 == 0        
        if (GameLoop.SkipFrame(frameOrder.stamina))
            return;

        if (PlayerInfo.GetPassedStaminaTime() >= 15)
            PlayerInfo.RecoverStamina();
    }
    
    float deadLimit = 900f;
    void Action_CheckDead(){
        if (GameLoop.SkipFrame(frameOrder.dead))
            return;
        
        if (SnailStat.CheckDead(deadLimit))
        {
            actionManager.DoCreatureAction(CreatureState.dead);
            PlayerInfo.isDead = true;
        }
    }

    float evolveLimit = 4320f;
    void Action_CheckEvolve(){
        if (GameLoop.SkipFrame(frameOrder.evolve))
            return;
        
        if (!PlayerInfo.canEveolve &&
            PlayerInfo.GetPassedCreatureInitTime() > evolveLimit)
            PlayerInfo.canEveolve = true;
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

    // ******* Conditional Action *******

    void RegistConditionConsumeAction()
    {
        actionManager.RegistConditionConsumeAction(ConditionCheckType.coin, UseCondition);
        actionManager.RegistConditionConsumeAction(ConditionCheckType.stamina, UseCondition);
    }
    
    void UseCondition(ConditionCheckType type, int value)
    {
        if (type == ConditionCheckType.coin)
            PlayerInfo.UseCoin(value);
        else if (type == ConditionCheckType.stamina)
            PlayerInfo.UseStamina(value);
    }

    void RegistConditionAddAction()
    {
        actionManager.RegistConditionAddAction(ConditionCheckType.coin, AddCondition);
        actionManager.RegistConditionAddAction(ConditionCheckType.stamina, AddCondition);
    }

    void AddCondition(ConditionCheckType type, int value)
    {
        if (type == ConditionCheckType.coin)
            PlayerInfo.AddCoin(value);
        else if (type == ConditionCheckType.stamina)
            PlayerInfo.AddStamina(value);
    }

    void RegistConditionalAction()
    {
        actionManager.RegistConditionalAction(ConditionCheckType.coin, CheckCoin);
        actionManager.RegistConditionalAction(ConditionCheckType.stamina, CheckStamina);
        actionManager.RegistConditionalAction(ConditionCheckType.alive, CheckAlive);
        actionManager.RegistConditionalAction(ConditionCheckType.evolve, CheckEvelove);
    }

    bool CheckStamina(float needValue)
    {
        if (PlayerInfo.stamina >= (int)needValue)
            return true;
        return false;
    }
    
    bool CheckCoin(float needValue)
    {
        if (PlayerInfo.coin >= (int)needValue)
            return true;
        return false;
    }

    bool CheckAlive(float dummy)
    {
        if (PlayerInfo.isDead)
            return false;
        return true;
    }

    bool CheckEvelove(float dummy)
    {
        if (PlayerInfo.canEveolve)
            return true;
        return false;
    }

    // ******* Quit Action *******

    void RegistQuitAction()
    {
        actionManager.RegistQuitAction(PlayerInfo.SetLastLoginTime);
    }
}