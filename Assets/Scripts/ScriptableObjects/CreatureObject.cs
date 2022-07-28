using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using Unity.Notifications.Android;
using GoogleMobileAds.Api;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "snailStatusObject", menuName = "SCObjects/CreatureObject", order = 1)]
public class CreatureObject : ScriptableObject 
{
    [SerializeField]
    public List<SingleCreature> creatureList;

    public string creatureDataPath = "TempCaracter/SkeletonStatus";

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
