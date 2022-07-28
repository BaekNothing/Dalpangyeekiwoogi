using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "snailStatusObject", menuName = "SCObjects/PlayerInfoObject", order = 1)]
public class PlayerInfoObject : ScriptableObject 
{
    public int playerCreatureIndex;
    public string creatureName;
    public int coin;
    public int stamina;
    public int remainStaminaTime;
    public int remainAdTime;
    public bool notiAllow;

    public void Init()
    {
        playerCreatureIndex = PlayerPrefs.GetInt("IndexNumber");
        creatureName = "";
        coin = PlayerPrefs.GetInt("Coin");
        stamina = PlayerPrefs.GetInt("Stamina");
        remainStaminaTime = PlayerPrefs.GetInt("StaminaSettime");
        remainAdTime = PlayerPrefs.GetInt("ADTime");
        notiAllow = PlayerPrefs.GetInt("IsNotificatioAllow") == 1;
    }
}
