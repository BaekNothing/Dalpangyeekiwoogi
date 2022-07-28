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
using TMPro;

[CreateAssetMenu(fileName = "snailStatusObject", menuName = "SCObjects/SnailStatusObject", order = 1)]
public class SnailStatusObject : ScriptableObject {

    [Serializable]
    public struct snailStatus{
        public float hunber;
        public float dirt;
        public float happiness;
        public float health;
    }

}
