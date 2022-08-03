using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "snailStatusObject", menuName = "SCObjects/CreatureDataObject", order = 1)]
public class CreatureDataObject : ScriptableObject 
{
    public List<SingleCreature> creatureList;    
    string creatureDataPath = "TempCaracter/SkeletonStatus";
    
    public List<SkeletonDataAsset> skeletonDataAssetList = new List<SkeletonDataAsset>();
    
    public SkeletonDataAsset skeletonData_Dead;
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
        System.IO.DirectoryInfo dirs = new System.IO.DirectoryInfo(Application.dataPath + "/Resources/SkeletonData/Snails");
        foreach(var dir in dirs.GetDirectories())
            skeletonDataAssetList.Add(Resources.Load<SkeletonDataAsset>($"SkeletonData/Snails/{dir.Name}/skeleton_SkeletonData"));
        skeletonData_Dead = Resources.Load<SkeletonDataAsset>("SkeletonData/_DEAD/skeleton_SkeletonData");
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
