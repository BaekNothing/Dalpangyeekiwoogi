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
    private PlayerInfoObject playerInfo;
    public PlayerInfoObject PlayerInfo { get { return playerInfo; } }

    [SerializeField]
    private SnailStatusObject snailStat;
    public SnailStatusObject SnailStat { get { return snailStat; } }
    
    [SerializeField]
    private CreatureObject creature;
    public CreatureObject Creature { get { return creature; } }

    void Awake()
    {
        if(PlayerPrefs.HasKey($"{playerPerfOption.IndexNumber}")) 
            LoadDataFromPlayerPerfs();
        if(playerInfo.playerCreatureIndex < 0)
            SetCreature();    
        LoadCreatureData();
    }

    void SetCreature()
    {
        playerInfo.playerCreatureIndex = 0;
        snailStat.InitAllStat();
    }

    void LoadDataFromPlayerPerfs()
    {
        //This function for user who played old version of game
        //Load old Data just once
        if(PlayerPrefs.HasKey($"{playerPerfOption.isLoaded}") && 
            PlayerPrefs.GetInt($"{playerPerfOption.isLoaded}") == 1)
            return;
        
        playerInfo.Init();
        snailStat.ClearAllStat();
        
    }

    void LoadCreatureData()
    {
        //The list may be updated in the future, so initialize it at runtime
        creature.creatureList.Clear();
        List<Dictionary<string, object>> CreatureDatas = CSVReader.Read(creature.creatureDataPath);
        foreach (Dictionary<string, object> data in CreatureDatas)
            creature.creatureList.Add(new CreatureObject.SingleCreature().Creature_init(data));
        PlayerPrefs.SetInt($"{playerPerfOption.isLoaded}", 1);
    }

    readonly float tickCorrection = 0.02f; //= 1 / 900 * 20

    void Update()
    {
        // Action when frameCount % 20 == 0        
        int frameCount = Time.frameCount;
        int frameRemainder = (int)(frameCount * 0.05);
        if (frameCount - frameRemainder * 20 != 0) return;

        Debug.Log(Time.captureFramerate);

        snailStat.dirt.StatCalculateTick(Time.deltaTime * tickCorrection);
        snailStat.happiness.StatCalculateTick(Time.deltaTime * tickCorrection);
        snailStat.health.StatCalculateTick(Time.deltaTime * tickCorrection);
        snailStat.hunger.StatCalculateTick(Time.deltaTime * tickCorrection);
    }    
}