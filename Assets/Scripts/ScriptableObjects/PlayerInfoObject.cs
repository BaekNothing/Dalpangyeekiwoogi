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
        return (DateTime.Now - DateTime.Parse(staminaTime)).TotalMinutes;
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
            coin = 0;
            creatureList = new List<int>();
            for (int i = 0; i < _creature.creatureList.Count; i++)
                creatureList.Add(0);
        }
        
        creatureIndex = 0;
        creatureName = "";
        stamina = 100;
        creatureInitTime = DateTime.Now.ToString();
        staminaTime = DateTime.Now.ToString();
        notiAllow = true;
        canEveolve = false;
        isDead = false;
    }
}