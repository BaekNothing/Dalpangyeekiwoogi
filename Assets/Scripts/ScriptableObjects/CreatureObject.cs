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

[CreateAssetMenu(fileName = "snailStatusObject", menuName = "SCObjects/CreatureObject", order = 1)]
public class CreatureObject : ScriptableObject 
{
    public List<SingleCreature> creatureList;    
    string creatureDataPath = "TempCaracter/SkeletonStatus";
    
    public List<SkeletonDataAsset> skeletonDataAssetList = new List<SkeletonDataAsset>();
    string skeletonDataPath = "Assets/Resources/Snails/0_Basic/skeleton_SkeletonData";
    
    public void LoadCreatureData()
    {
        //The creature list may be scaledUp in the future, so initialize it at runtime
        creatureList.Clear();
        List<Dictionary<string, object>> CreatureDatas = CSVReader.Read(creatureDataPath);
        foreach (Dictionary<string, object> data in CreatureDatas)
            creatureList.Add(new CreatureObject.SingleCreature().Creature_init(data));
    }

    public void LoadSkeletonData(){
        skeletonDataAssetList.Clear();
        //Resources.LoadAll<SkeletonDataAsset>(skeletonDataPath);
        
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

        public string GetName() => values[0];
        public string GetDesc() => values[1];
    }
}
