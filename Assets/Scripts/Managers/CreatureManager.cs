using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using Consts;

public class CreatureManager : MonoBehaviour
{
    DataManager dataManager; 
    ActionManager actionManager;

    [SerializeField]
    GameObject creatureRootTransform;
    [SerializeField]
    SkeletonAnimation creature;
    SkeletonDataAsset skeletonDataAsset;
    [SerializeField]
    CreatureDataObject.SingleCreature creatureData;

    void Awake()
    {
        dataManager = this.GetComponent<DataManager>();
        actionManager = this.GetComponent<ActionManager>();

        RegistInitAction();
        RegistCreatureAction();
        RegistTickAction();

        actionManager.initFlag[nameof(CreatureManager)] = true;
    }

    void RegistInitAction(){
        actionManager.RegistInitAction(()=>LoadCreature(
            dataManager.PlayerInfo.isDead,
            dataManager.PlayerInfo.creatureIndex
        ));
    }

    public void LoadCreature(bool isDead, int index)
    {
        if(creature)
                Destroy(creature);
        if(!isDead)
        {
            creatureData = dataManager.Creature.creatureList[index];
            skeletonDataAsset = dataManager.Creature.skeletonDataAssetList[index];
            creature = SkeletonAnimation.NewSkeletonAnimationGameObject(skeletonDataAsset);
            creature.name = $"Creature_{index}";
            creature.AnimationState.SetAnimation(0, CreatureState.stand.ToString(), true).TimeScale = 1f;
            creature.transform.Translate(creatureRootTransform.transform.position);
        }
        else 
            creature = SkeletonAnimation
                .NewSkeletonAnimationGameObject(dataManager.Creature.skeletonData_Dead);
    }

    void RegistCreatureAction(){
        actionManager.RegistCreatureAction(SetCreatureAnimation);
    }

    float deadLimit = 900f;
    float evolveLimit = 4320f;
    public void RegistTickAction()
    {
        actionManager.RegistTickAction(
            ()=>{
                // DeadCheck;
                foreach(StatusType type in StatusType.GetValues(typeof(StatusType)))
                    if(dataManager.SnailStat.
                        GetStatusDeadTime(type) > deadLimit)
                        CreatureDead();
            }
        );
        
        actionManager.RegistTickAction(
            ()=>{
                // EvolveCheck;
                if (dataManager.PlayerInfo.GetPassedCreatureInitTime() > evolveLimit)
                    CreatureEvolve();
            }
        );
    }
    
    public void CreatureDead()
    {
        if(dataManager.PlayerInfo.isDead) return;
        dataManager.PlayerInfo.creatureIndex = 0;
        dataManager.PlayerInfo.SetCreature(0, 1);
        LoadCreature(true, 0);
        dataManager.PlayerInfo.isDead = true;

        ComponentUtility.Log("DEAD");
    }

    public void CreatureEvolve()
    {
        dataManager.PlayerInfo.SetCreatureInitTime();
        int newIndex = UnityEngine.Random.Range(0, dataManager.Creature.creatureList.Count);
        dataManager.PlayerInfo.creatureIndex = newIndex;
        dataManager.PlayerInfo.SetCreature(newIndex, 1);
        LoadCreature(false, newIndex);
        ComponentUtility.Log("EVOLVE");
    }

    public void SetCreatureAnimation(CreatureState state)
    {
        //NO PLAY Animation
        if (state == CreatureState.Clean)
            state = CreatureState.Play;

        StopCoroutine(SetAnimation(state));
        StartCoroutine(SetAnimation(state));
    }

    WaitForSeconds wait = new WaitForSeconds(2f);
    IEnumerator SetAnimation(CreatureState state){
        creature.AnimationState.SetAnimation(0, state.ToString(), true).TimeScale = 1f;
        yield return wait;
        creature.AnimationState.SetAnimation(0, CreatureState.stand.ToString(), true).TimeScale = 1f;
    }
}
