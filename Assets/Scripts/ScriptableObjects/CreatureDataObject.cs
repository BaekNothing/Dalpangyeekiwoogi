using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using Unity.Notifications.Android;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "snailStatusObject", menuName = "SCObjects/CreatureDataObject", order = 1)]
public class CreatureDataObject : ScriptableObject 
{
    public List<SingleCreature> creatureList;    
    string creatureDataPath = "TempCaracter/SkeletonStatus";
    
    public List<SkeletonDataAsset> skeletonDataAssetList = new List<SkeletonDataAsset>();
    
    public void LoadCreatureData()
    {
        //The creature list may be scaledUp in the future, so initialize it at runtime
        creatureList.Clear();
        List<Dictionary<string, object>> CreatureDatas = CSVReader.Read(creatureDataPath);
        foreach (Dictionary<string, object> data in CreatureDatas)
            creatureList.Add(new CreatureDataObject.SingleCreature().Creature_init(data));
    }

    public void LoadSkeletonData()
    {
        skeletonDataAssetList.Clear();
        System.IO.DirectoryInfo dirs = new System.IO.DirectoryInfo(Application.dataPath + "/Resources/Snails");
        foreach(var dir in dirs.GetDirectories())
            skeletonDataAssetList.Add(Resources.Load<SkeletonDataAsset>($"Snails/{dir.Name}/skeleton_SkeletonData"));
    }

    [Serializable]
    public struct SingleCreature
    {
        [SerializeField]
        string[] values;
        
        public SingleCreature Creature_init(Dictionary<string, object> values)
        {
            var objValues = values.Values.ToArray();
            this.values = new string[objValues.Length];
            for (int i = 0; i < objValues.Length; i++)
                this.values[i] = objValues[i].ToString();
            return this;
        }

        public string GetName() => values[1];
        public string GetDesc() => values[2];
    }
}
