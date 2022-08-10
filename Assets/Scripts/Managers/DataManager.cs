using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using Consts;
using System.Linq;


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
        //Creature.LoadCreatureData();
        //Creature.LoadSkeletonData();
        if(!PlayerInfo.isLoaded) 
        {
            PlayerInfo.CheckLegacyPrefs(_creature);
            SnailStat.InitAllStat(900f, 1f);
            SnailStat.ClearAllStat();
        }

        actionManager = this.GetComponent<ActionManager>();
        RegistInitAction();
        RegistTickAction();
        RegistPlayerInfoAction();
        RegistStatusAction();
        RegistQuitAction();
        RegistConditionalAction();
        RegistConditionConsumeAction();
        RegistConditionAddAction();

        actionManager.initFlag[nameof(DataManager)] = true;
    }

    void RegistInitAction()
    {
        actionManager.RegistInitAction(CheckDealiedStat);
    }

    void CheckDealiedStat()
    {
        double passedMin = PlayerInfo.GetPassedLoginTime();
        if (passedMin > 5)
        {
            ComponentUtility.Log($"CheckDealiedStat : {PlayerInfo.GetPassedLoginTime()} : {PlayerInfo.lastLoginTime} : {DateTime.Now}");
            Dictionary<string, float> prevStat = SnailStat.GetAllStat();
            CalculateDelaiedStat(passedMin);
            Dictionary<string, float> curStat = SnailStat.GetAllStat();
            string resultStr = MakeResultStr(prevStat, curStat);

            actionManager.DoUIPnlShowAction("revisit",  new List<UIPanels.textFactor>{
                new UIPanels.textFactor(
                    "title",
                    $"{(int)passedMin}분 만에 돌아오셨군요!"
                ),
                new UIPanels.textFactor(
                    "desc",
                    resultStr
                )
            });
        }
    }

    void CalculateDelaiedStat(double passedMin)
    {
        for (int i = 0; i < passedMin * 60; i++)
        {
            //decrease stat 0.001f per second
            SnailStat.CalculateTickAllStat(0.001f);
            Action_CheckEvolve();
            Action_CheckDead();
        }
    }

    string MakeResultStr(Dictionary<string, float> prev, Dictionary<string, float> cur)
    {
        string resultStr = "당신의 달팽이는\n";
        var keys = prev.Keys.ToArray();
        int ManyOrLess = 70;
        int LessOrNone = 20;

        foreach(string key in keys)
        {
            int diff = (int)(prev[key] - cur[key]);
            if (cur[key] == 0)
                resultStr += $"심각하게 {key}\n";
            else if(diff > ManyOrLess)
                resultStr += $"매우 {key}\n";
            else if (diff > LessOrNone)
                resultStr += $"{key}\n";
            else
                resultStr += $"별로 안 {key}\n";
        }

        return resultStr;
    
    }

    // ****** Tick Action *******

    void RegistTickAction()
    {
        actionManager.RegistTickAction(Action_CalcualteStat);
        actionManager.RegistTickAction(Action_CalculateStamina);
        actionManager.RegistTickAction(Action_CheckDead);
        actionManager.RegistTickAction(Action_CheckEvolve);
        actionManager.RegistTickAction(Action_SetLastlogin);
    }

    void Action_CalcualteStat()
    {
        // Action when frameCount % 20 == 0        
        if (GameLoop.SkipFrame(frameOrder.stat))
            return;
        SnailStat.CalculateTickAllStat(Time.deltaTime * GameLoop.tickCorrection);
    }

    void Action_CalculateStamina()
    {
        // Action when frameCount % 20 == 0        
        if (GameLoop.SkipFrame(frameOrder.stamina))
            return;

        if (PlayerInfo.GetPassedStaminaTime() >= GameLoop.staminaLimitTime)
            PlayerInfo.RecoverStamina();
    }
    
    void Action_CheckDead(){
        if (GameLoop.SkipFrame(frameOrder.dead))
            return;
        
        if (SnailStat.CheckDead(GameLoop.deadLimit))
        {
            actionManager.DoCreatureAction(CreatureActionType.dead);
            PlayerInfo.isDead = true;
        }
    }

    void Action_CheckEvolve(){
        if (GameLoop.SkipFrame(frameOrder.evolve))
            return;
        
        if (!PlayerInfo.canEveolve &&
            PlayerInfo.GetPassedCreatureInitTime() > GameLoop.evolveLimit)
            PlayerInfo.canEveolve = true;
    }

    void Action_SetLastlogin()
    {
        if (GameLoop.SkipFrame(frameOrder.refresh))
            return;    
        PlayerInfo.SetLastLoginTime();
    }

    // ****** PlayerInfo Action *******
    void RegistPlayerInfoAction(){
        actionManager.RegistPlayerInfoAction(PlayerInfoActionType.initPlayerInfo, InitPlayerInfo);
        actionManager.RegistPlayerInfoAction(PlayerInfoActionType.getStaminaRemainTime, GetStaminaRemainTime);
        actionManager.RegistPlayerInfoAction(PlayerInfoActionType.getStamina, GetStamina);
        actionManager.RegistPlayerInfoAction(PlayerInfoActionType.getCoin, GetCoin);
        actionManager.RegistPlayerInfoAction(PlayerInfoActionType.getCreatureData, GetCreatureData);
        actionManager.RegistPlayerInfoAction(PlayerInfoActionType.getPlayerName, GetPlayerName);
        actionManager.RegistPlayerInfoAction(PlayerInfoActionType.setPlayerName, SetPlayerName);
    }

    string InitPlayerInfo(string newIndex){       
        PlayerInfo.ClearAllData();
        PlayerInfo.creatureIndex = int.Parse(newIndex ?? "0");
        PlayerInfo.SetCreature(int.Parse(newIndex ?? "0"), 1);
        PlayerInfo.RecoverStamina();

        SnailStat.ClearAllStat();
        SnailStat.InitAllStat(900f, 1f);
        return $"{PlayerInfoActionType.initPlayerInfo} : {newIndex}";
    }

    string GetStaminaRemainTime(string value){
        return ((int)(GameLoop.staminaLimitTime - 
                PlayerInfo.GetPassedStaminaTime())).ToString();
    }

    string GetStamina(string value){
        return (PlayerInfo.GetStamina()).ToString();
    }

    string GetCoin(string value){
        return (PlayerInfo.GetCoin()).ToString();
    }

    string GetCreatureData(string value){
        int index = int.Parse(value ?? "0");
        var creatureList = Creature.creatureList;
        if (index < 0 || index >= creatureList.Count)
            return $"no Creature Data {index}";

        CreatureDataObject.SingleCreature creature = creatureList[index];
        return $"{creature.GetName()} ^ {creature.GetDesc()}";
    }

    string GetPlayerName(string value){
        return PlayerInfo.creatureName;
    }

    string SetPlayerName(string value){
        PlayerInfo.creatureName = value;
        return $"{PlayerInfoActionType.setPlayerName} : {value}";
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
        actionManager.RegistConditionalAction(ConditionCheckType.creatureList, CheckCreatureList);
        actionManager.RegistConditionalAction(ConditionCheckType.coin, CheckCoin);
        actionManager.RegistConditionalAction(ConditionCheckType.stamina, CheckStamina);
        actionManager.RegistConditionalAction(ConditionCheckType.alive, CheckAlive);
        actionManager.RegistConditionalAction(ConditionCheckType.evolve, CheckEvelove);
        actionManager.RegistConditionalAction(ConditionCheckType.dirt, CheckDirt);
    }

    bool CheckCreatureList(float index){
        return PlayerInfo.GetCreature((int)index) > 0;
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
        return !PlayerInfo.isDead;
    }

    bool CheckEvelove(float dummy)
    {
        return PlayerInfo.canEveolve;
    }
    
    bool CheckDirt(float compareValue)
    {
        return SnailStat.GetStatusValue(StatusType.dirt) >= compareValue;
    }


    // ******* Quit Action *******

    void RegistQuitAction()
    {
        actionManager.RegistQuitAction(PlayerInfo.SetLastLoginTime);
    }

    // ******* Debug Action *******

    public void ClearAllData()
    {
        PlayerInfo.ClearAllData();
        SnailStat.ClearAllStat();
        actionManager.DoEvolve(0);
        //PlayerInfo.AddCoin(30);
    }
}
