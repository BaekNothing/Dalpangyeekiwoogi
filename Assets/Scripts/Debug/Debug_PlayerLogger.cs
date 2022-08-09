using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Consts;

public class Debug_PlayerLogger : MonoBehaviour
{
    [SerializeField]
    Text lblStat;

    [SerializeField]
    PlayerInfoObject playerInfo;

    // Update is called once per frame
    void Update()
    {
        lblStat.text =
            "PlayerLogger : \n" +
            $"{MakeSingleStatusToString(playerInfo)}";
           
    }

    string MakeSingleStatusToString(PlayerInfoObject playerInfo)
    {
        string result = "";
        result += $"lastLoginTime : {playerInfo.lastLoginTime}\n";
        result += $"creatureIndex : {playerInfo.creatureIndex}\n";
        result += $"creatureName : {playerInfo.creatureName}\n";
        result += $"creatureInitTime : {playerInfo.creatureInitTime}\n";
        result += $"stamina : {playerInfo.stamina}\n";
        result += $"staminaTime : {playerInfo.staminaTime}\n";

        return result;
    }
}
