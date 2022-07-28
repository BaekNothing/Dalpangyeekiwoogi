using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public SnailStatusObject hunger;
    public SnailStatusObject dirt;
    public SnailStatusObject happy;
    public SnailStatusObject health;

    public CreatureObject creature;

    void Start()
    {
        hunger.status.StatInit();
        dirt.status.StatInit();
        happy.status.StatInit();
        health.status.StatInit();

        creature.status.Clear();
        List<Dictionary<string, object>> CreatureDatas 
            = CSVReader.Read("TempCaracter/SkeletonStatus");
        foreach (Dictionary<string, object> data in CreatureDatas)
            creature.status.Add
                (new CreatureObject.SingleCreature().Creature_init(data));
    }
}
