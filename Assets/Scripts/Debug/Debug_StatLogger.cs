using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Consts;

public class Debug_StatLogger : MonoBehaviour
{
    [SerializeField]
    Text lblStat; 

    [SerializeField]
    SnailStatusObject statObj;
    
    // Update is called once per frame
    void Update()
    {
        lblStat.text = 
            "StatLogger : \n" +
            $"{MakeSingleStatusToString(StatusType.dirt)}" +
            $"{MakeSingleStatusToString(StatusType.happiness)}" +
            $"{MakeSingleStatusToString(StatusType.health)}" +
            $"{MakeSingleStatusToString(StatusType.hunger)}";
    }

    string MakeSingleStatusToString(StatusType type)
    {
        return $"{type} : {statObj.GetStatusValue(type)} {statObj.GetStatusDeadTime(type)} {statObj.GetDeadCount(type)}\n";
    } 
}
