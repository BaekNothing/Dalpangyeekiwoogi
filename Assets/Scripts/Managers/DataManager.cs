using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    enum playerPerfOption
    {
        IndexNumber,
        isLoaded,
        playTime
    }

    [SerializeField]
    private PlayerInfoObject _playerInfo;
    public PlayerInfoObject PlayerInfo { get { return _playerInfo; } }

    [SerializeField]
    private SnailStatusObject _snailStat;
    public SnailStatusObject SnailStat { get { return _snailStat; } }
    
    [SerializeField]
    private CreatureObject _creature;
    public CreatureObject Creature { get { return _creature; } }

    void Awake()
    {
        if(PlayerPrefs.HasKey($"{playerPerfOption.IndexNumber}")) 
            LoadDataFromPlayerPerfs();
        if(_playerInfo.playerCreatureIndex < 0)
            SetCreature();    
        LoadCreatureData();
    }

    // ****** Data *******

    void SetCreature()
    {
        _playerInfo.playerCreatureIndex = 0;
        _snailStat.InitAllStat();
    }

    void LoadDataFromPlayerPerfs()
    {
        //This function for user who played old version of game
        //Load old Data just once
        if(PlayerPrefs.HasKey($"{playerPerfOption.isLoaded}") && 
            PlayerPrefs.GetInt($"{playerPerfOption.isLoaded}") == 1)
            return;
        
        _playerInfo.Init();
        _snailStat.ClearAllStat();
        
    }

    void LoadCreatureData()
    {
        //The list may be updated in the future, so initialize it at runtime
        _creature.creatureList.Clear();
        List<Dictionary<string, object>> CreatureDatas = CSVReader.Read(_creature.creatureDataPath);
        foreach (Dictionary<string, object> data in CreatureDatas)
            _creature.creatureList.Add(new CreatureObject.SingleCreature().Creature_init(data));
        PlayerPrefs.SetInt($"{playerPerfOption.isLoaded}", 1);
    }

    // ****** Action *******

    public List<registedAction> actionList = new List<registedAction>();
    
    public struct registedAction
    {
        public SnailStatusObject.SingleStatus target;
        public condition condition;
        public float value;
        public System.Action action;

        registedAction(SnailStatusObject.SingleStatus target, condition condition, float value, System.Action action)
        {
            this.target = target;
            this.condition = condition;
            this.value = value;
            this.action = action;
        }
    }

    public enum condition { isBigger, isSmaller,isEqual }

    void DoRegistedAction(registedAction inputAction)
    {
        //much more memory but more readable
        SnailStatusObject.SingleStatus target = inputAction.target;
        condition condition = inputAction.condition;
        float value = inputAction.value;
        System.Action action = inputAction.action;

        if (condition == condition.isBigger)
            if (target.value > value)
                action();
        else if (condition == condition.isSmaller)
            if (target.value < value)
                action();
        else if (condition == condition.isEqual)
            if (target.value == value)
                action();
    }

    readonly float tickCorrection = 0.02f; //= 1 / 900 * 20
    public void CalcualteData_EveryTick()
    {
        // Action when frameCount % 20 == 0        
        int frameCount = Time.frameCount;
        int frameRemainder = (int)(frameCount * 0.05);
        if (frameCount - frameRemainder * 20 != 0) return;

        _snailStat.CalculateTickAllStat(Time.deltaTime * tickCorrection);

        foreach (registedAction action in actionList)
            DoRegistedAction(action);
    }

    void Update() => CalcualteData_EveryTick();
}