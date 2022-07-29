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

    ActionManager actionManager;
    void Start()
    {
        actionManager = this.GetComponent<ActionManager>();
        actionManager.RegistTickAction(Action_CalcualteData);
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
    
    readonly float tickCorrection = 0.02f; //= 1 / 900 * 20
    public void Action_CalcualteData()
    {
        // Action when frameCount % 20 == 0        
        int frameCount = Time.frameCount;
        int frameRemainder = (int)(frameCount * 0.05);
        if (frameCount - frameRemainder * 20 != 0) return;

        SnailStat.CalculateTickAllStat(Time.deltaTime * tickCorrection);       
    }
}