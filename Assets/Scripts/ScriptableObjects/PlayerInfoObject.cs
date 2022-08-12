using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "snailStatusObject", menuName = "SCObjects/PlayerInfoObject", order = 1)]
public class PlayerInfoObject : ScriptableObject 
{
    public bool isLoaded;

    public string lastLoginTime;
    public int creatureIndex;
    public string creatureInitTime;
    public string creatureName;
    public bool isDead;
    public bool canEveolve;

    public int coin;
    public int stamina;
    public string staminaTime;

    [SerializeField]
    List<int> creatureList = new List<int>();
    
    //****** Creature *******
    public void SetCreature(int index, int value)
    {
        while (index >= creatureList.Count)
            creatureList.Add(0);
        creatureList[index] = value;
    }

    public int GetCreature(int index){
        while (index >= creatureList.Count)
            creatureList.Add(0);
        return creatureList[index];
    }

    //****** Last Login *******
    public void SetLastLoginTime()
    {
        lastLoginTime = DateTime.Now.ToString();
    }

    public double GetPassedLoginTime()
    {   
        if (lastLoginTime == null)
            lastLoginTime = DateTime.Now.ToString();
        return (DateTime.Now - DateTime.Parse(lastLoginTime)).TotalMinutes;
    }

    //****** Creature Init Time *******
    public void SetCreatureInitTime()
    {
        creatureInitTime = DateTime.Now.ToString();
    }

    public double GetPassedCreatureInitTime()
    {
        if (creatureInitTime == null)
            creatureInitTime = DateTime.Now.ToString();
        return (DateTime.Now - DateTime.Parse(creatureInitTime)).TotalMinutes;
    }

    //****** Stamina *******
    public int RecoverStamina()
    {
        stamina = 100;
        staminaTime = DateTime.Now.ToString();
        return stamina;
    }

    public int SetStamina(int value)
    {
        stamina = value;
        if (stamina < 0)
            stamina = 0;
        if (stamina > 100)
            stamina = 100;
        return stamina;
    }

    public int AddStamina(int value)
    {
        stamina += value;
        if (stamina > 100)
            stamina = 100;
        return stamina;
    }

    public int UseStamina(int value)
    {
        stamina -= value;
        if (stamina < 0)
            stamina = 0;
        return stamina;
    }

    public int GetStamina()
    {
        return stamina;
    }

    public double GetPassedStaminaTime(){
        if (staminaTime == null)
            staminaTime = DateTime.Now.ToString();
        
        double time = (DateTime.Now - DateTime.Parse(staminaTime)).TotalMinutes;
        //ComponentUtility.Log($"{time} : {staminaTime} : {DateTime.Now}");
        if(time < 0)
        {
            staminaTime = DateTime.Now.ToString();
            time = 0;
        }
        return time;
    }

    //****** COIN *******
    public int SetCoin(int value){
        coin = value;
        if (coin < 0)
            coin = 0;
        return coin;
    }

    public int AddCoin(int value){
        coin += value;
        return coin;
    }

    public int GetCoin(){
        return coin;
    }

    public int UseCoin(int value){
        coin -= value;
        if (coin < 0)
            coin = 0;
        return coin;
    }

    //****** Load from Legacy *******
    public void CheckLegacyPrefs(CreatureDataObject _creature)
    {
        isLoaded = true;
        if (PlayerPrefs.HasKey("IndexNumber"))
        {
            try
            {
                coin = PlayerPrefs.GetInt("Coin");
                if (coin < 30)
                    coin = 30;
                creatureList = new List<int>();
                for (int i = 0; i < _creature.creatureList.Count; i++)
                    if (PlayerPrefs.HasKey($"Creature{i}"))
                        creatureList.Add(PlayerPrefs.GetInt($"Creature{i}"));
                    else
                        creatureList.Add(0);
                // load data from legacy PlayerPerfs       
                
            }catch (System.Exception e) {
                Debug.Log(e.Message);}
        }
        else
        {
            coin = 30;
            creatureList = new List<int>();
            for (int i = 0; i < _creature.creatureList.Count; i++)
                creatureList.Add(0);
        }
        creatureList[0] = 1;
        creatureName = "±×³É ´ÞÆØÀÌ";
        ClearAllData();
    }

    public void ClearAllData(){
        creatureIndex = 0;
        SetCreatureInitTime();
        SetLastLoginTime();
        RecoverStamina();
        canEveolve = false;
        isDead = false;
        SavetoJson();
    }

    //string filePath = "/Resources/SCObjs/PlayerInfoObject.json";
    public void SavetoJson()
    {
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("PlayerInfoObject", json);
        //System.IO.File.WriteAllText(Application.dataPath + "/Resources/SCObjs/PlayerInfoObject.json", json);
    }

    public void LoadfromJson()
    {
        //string json = System.IO.File.ReadAllText(Application.dataPath + "/Resources/SCObjs/PlayerInfoObject.json");
        string json = PlayerPrefs.GetString("PlayerInfoObject");
        JsonUtility.FromJsonOverwrite(json, this);
    }

    // public void SavetoJson(){
    //     string json = JsonUtility.ToJson(this);
    //     PlayerPrefs.SetString("PlayerInfo", json);
    // }

    // public void LoadFromJson(){
    //     string json = PlayerPrefs.GetString("PlayerInfo");
    //     PlayerInfoObject playerInfo = JsonUtility.FromJson<PlayerInfoObject>(json);
    //     isLoaded = true;
    //     coin = playerInfo.coin;
    //     creatureList = playerInfo.creatureList;
    //     creatureIndex = playerInfo.creatureIndex;
    //     creatureInitTime = playerInfo.creatureInitTime;
    //     stamina = playerInfo.stamina;
    //     staminaTime = playerInfo.staminaTime;
    //     lastLoginTime = playerInfo.lastLoginTime;
    //     creatureName = playerInfo.creatureName;
    //     canEveolve = playerInfo.canEveolve;
    //     isDead = playerInfo.isDead;
    // }
}
