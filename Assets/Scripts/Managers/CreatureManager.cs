using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using Consts;
using System;

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
        RegistEvolveAction();

        actionManager.initFlag[nameof(CreatureManager)] = true;
    }

    void RegistInitAction(){
        actionManager.RegistInitAction(()=>
            LoadCreature(
                dataManager.PlayerInfo.isDead,
                dataManager.PlayerInfo.creatureIndex
            )
        );
    }

    void LoadCreature(bool isDead, int index)
    {
        if(creature)
        {
            Destroy(creature.gameObject);
            creature = null;
        }
        if(!isDead)
        {
            creatureData = dataManager.Creature.creatureList[index];
            skeletonDataAsset = dataManager.Creature.skeletonDataAssetList[index];
            creature = SkeletonAnimation.NewSkeletonAnimationGameObject(skeletonDataAsset);
            creature.name = $"Creature_{index}";
            creature.AnimationState.SetAnimation(0, CreatureActionType.stand.ToString(), true).TimeScale = 1f;
            creature.transform.Translate(creatureRootTransform.transform.position);
        }
        else 
            creature = SkeletonAnimation
                .NewSkeletonAnimationGameObject(dataManager.Creature.skeletonData_Dead);
    }

    void RegistCreatureAction(){
        actionManager.RegistCreatureAction(SetCreatureActionType);
    }

    void SetCreatureActionType(CreatureActionType actionType, int value)
    {
        if(dataManager.PlayerInfo.isDead) return;

        switch(actionType)
        {
            case CreatureActionType.stand:
                StopCoroutine(SetAnimation(actionType, value));
                StartCoroutine(SetAnimation(actionType, value));
                break;
            case CreatureActionType.Play:
                StopCoroutine(SetAnimation(actionType, value));
                StartCoroutine(SetAnimation(actionType, value));
                break;
            case CreatureActionType.Clean:
                actionType = CreatureActionType.Play;
                StopCoroutine(SetAnimation(actionType, value));
                StartCoroutine(SetAnimation(actionType, value));
                break;
            case CreatureActionType.Eat:
                StopCoroutine(SetAnimation(actionType, value));
                StartCoroutine(SetAnimation(actionType, value));
                break;
            case CreatureActionType.dead:
                CreatureDead();
                break;
            case CreatureActionType.evolve:
                CreatureEvolve(value);
                break;
        }
    }

    IEnumerator SetAnimation(CreatureActionType actionType, int animationTime = 0){
        creature.AnimationState.SetAnimation(0, actionType.ToString(), true).TimeScale = 1f;
        yield return new WaitForSeconds(animationTime);
        creature.AnimationState.SetAnimation(0, CreatureActionType.stand.ToString(), true).TimeScale = 1f;
    }
    
    void CreatureDead()
    {
        LoadCreature(true, 0);
        ComponentUtility.Log("DEAD");
    }
    
    public void CreatureEvolve(int newIndex = -1)
    {
        if(dataManager.PlayerInfo.isDead) return;
        if(newIndex == -1) newIndex = GetNewEvolveIndex();
        ComponentUtility.Log($"EVOLVE {newIndex}");
        actionManager.DoPlayerInfoAction
            (PlayerInfoActionType.initPlayerInfo, newIndex.ToString());

        LoadCreature(false, newIndex);
    }

    int GetNewEvolveIndex(){

        int whileBreaker = 0;
        while (whileBreaker++ < 100)
        {
            int newIndex = UnityEngine.Random
                .Range(0, dataManager.Creature.skeletonDataAssetList.Count - 1);
            if (dataManager.PlayerInfo.creatureIndex == newIndex) continue;
            return newIndex;
        }
        return 0;
    }

    void RegistEvolveAction(){
        actionManager.RegistEvolveAction(CreatureEvolve_Force);
    }

    public void CreatureEvolve_Force(int newIndex = -1)
    {
        if (newIndex == -1) newIndex = GetNewEvolveIndex();
        ComponentUtility.Log($"EVOLVE {newIndex}");
        actionManager.DoPlayerInfoAction
            (PlayerInfoActionType.initPlayerInfo, newIndex.ToString());

        LoadCreature(false, newIndex);
    }

}
