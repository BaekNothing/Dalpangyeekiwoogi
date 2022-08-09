using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Consts;

public class Debug_CreatureLogger : MonoBehaviour
{
    [SerializeField]
    Text lblStat;

    [SerializeField]
    CreatureDataObject creatureInfo;

    // Update is called once per frame
    void Update()
    {
        lblStat.text =
            "PlayerLogger : \n" +
            $"{MakeSingleStatusToString(creatureInfo)}";

    }

    string MakeSingleStatusToString(CreatureDataObject creatureInfo)
    {
        string result = "";
        result += $"creatureList : ";
        foreach(var creature in creatureInfo.creatureList)
            result += $"{creature.GetName()}\n";
        result += "\n";
        result += "skeletonDataAssetList : ";
        foreach(var skeletonDataAsset in creatureInfo.skeletonDataAssetList)
            result += $"{skeletonDataAsset.name}\n";
        result += "\n";
        result += $"skeletonDataDead : {creatureInfo.skeletonData_Dead.name}";
        result += "\n";
        return result;
    }
}
