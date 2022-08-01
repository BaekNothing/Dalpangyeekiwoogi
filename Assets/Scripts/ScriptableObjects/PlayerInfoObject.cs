using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "snailStatusObject", menuName = "SCObjects/PlayerInfoObject", order = 1)]
public class PlayerInfoObject : ScriptableObject 
{
     public bool isLoaded;

    public DateTime lastLoginTime;
    public int creatureIndex;
    DateTime creatureInitTime;
    public string creatureName;
    public int coin;
    public int stamina;
    DateTime staminaTime;
    DateTime ADTime;
    public bool notiAllow;

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
        lastLoginTime = DateTime.Now;
    }

    public double GetPassedLoginTime()
    {   
        if (lastLoginTime == null)
            lastLoginTime = DateTime.Now;
        return (DateTime.Now - lastLoginTime).TotalMinutes;
    }

    //****** Creature Init Time *******
    public void SetCreatureInitTime()
    {
        creatureInitTime = DateTime.Now;
    }

    public double GetPassedCreatureInitTime()
    {
        if (creatureInitTime == null)
            creatureInitTime = DateTime.Now;
        return (DateTime.Now - creatureInitTime).TotalMinutes;
    }

    //****** Stamina *******
    public int RecoverStamina(){
        stamina = 100;
        staminaTime = DateTime.Now;
        return stamina;
    }
    public int SetStamina(int value){
        stamina = value;
        return stamina;
    }
    public int UseStamina(int value){
        stamina -= value;
        if (stamina < 0)
            stamina = 0;
        return stamina;
    }
    public int GetStamina(){
        return stamina;
    }

    public double GetPassedStaminaTime(){
        if (staminaTime == null)
            staminaTime = DateTime.Now;
        return (DateTime.Now - staminaTime).TotalMinutes;
    }

    //****** Load from Legacy *******
    public void CheckLegacyPrefs(CreatureDataObject _creature)
    {
        isLoaded = true;
        if (!PlayerPrefs.HasKey("IndexNumber"))
            return;
        try
        {
            // load data from legacy PlayerPerfs       
            creatureIndex = PlayerPrefs.GetInt("IndexNumber");
            creatureName = "";
            coin = PlayerPrefs.GetInt("Coin");
            stamina = PlayerPrefs.GetInt("Stamina");
            staminaTime = DateTime.Now;
            ADTime = DateTime.Now;
            notiAllow = PlayerPrefs.GetInt("IsNotificatioAllow") == 1;
            creatureList = new List<int>();
            for (int i = 0; i < _creature.creatureList.Count; i++)
                if(PlayerPrefs.HasKey($"Creature{i}"))
                    creatureList.Add(PlayerPrefs.GetInt($"Creature{i}"));
                else
                    creatureList.Add(0);
        }catch (System.Exception e) {
            Debug.Log(e.Message);}
    }
}