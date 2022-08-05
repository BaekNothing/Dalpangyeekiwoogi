using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Consts;


[RequireComponent(typeof(Slider))]
public class StatSlider : MonoBehaviour
{
    [SerializeField]
    SnailStatusObject statData;

    [SerializeField]
    StatusType statType;

    Slider stat;



    void Awake()
    {
        stat = GetComponent<Slider>();
        if (statData == null)
            ComponentUtility.LogError("statData is null");
    }

    // Update is called once per frame
    void Update()
    {
        if (GameLoop.SkipFrame(frameOrder.refresh)) return;
        stat.value = statData.GetStatusValue(statType);
    }
}
