using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

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
    public enum CreatureState {Eat, Play, stand}

    void Awake()
    {
        dataManager = this.GetComponent<DataManager>();
        actionManager = this.GetComponent<ActionManager>();
        //AnimationState.SetAnimation(0, "stand", true).TimeScale = 1f;
        actionManager.RegistInitAction(LoadCreature);
        actionManager.initFlag[nameof(CreatureManager)] = true;
    }

    void RegistStateAction(){
        actionManager.RegistCreatureAction(CreatureState.Eat, ()=>{
            creature.AnimationState.SetAnimation(0, CreatureState.Eat.ToString(), true);
        });
        actionManager.RegistCreatureAction(CreatureState.Play, ()=>{
            creature.AnimationState.SetAnimation(0, CreatureState.Play.ToString(), true);
        });
        actionManager.RegistCreatureAction(CreatureState.stand, ()=>{
            creature.AnimationState.SetAnimation(0, CreatureState.stand.ToString(), true);
        });
    }

    public void LoadCreature()
    {
        int index = dataManager.PlayerInfo.creatureIndex;
        if(creature)
            Destroy(creature);
        creatureData = dataManager.Creature.creatureList[index];
        skeletonDataAsset = dataManager.Creature.skeletonDataAssetList[index];
        creature = SkeletonAnimation.NewSkeletonAnimationGameObject(skeletonDataAsset);
        creature.name = $"Creature_{index}";
        creature.AnimationState.SetAnimation(0, CreatureState.stand.ToString(), true).TimeScale = 1f;
        creature.transform.Translate(creatureRootTransform.transform.position);
    }
    
    public void CreatureDead()
    {

    }

    public void SetCreatureAnimation(CreatureState state)
    {
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
