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

    void LoadCreature(bool isDead, int index)
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
        actionManager.RegistCreatureAction(SetCreatureState);
    }

    void SetCreatureState(CreatureState state)
    {
        if(dataManager.PlayerInfo.isDead) return;

        switch(state)
        {
            case CreatureState.stand:
                StopCoroutine(SetAnimation(state));
                StartCoroutine(SetAnimation(state));
                break;
            case CreatureState.Play:
                StopCoroutine(SetAnimation(state));
                StartCoroutine(SetAnimation(state));
                break;
            case CreatureState.Clean:
                state = CreatureState.Play;
                StopCoroutine(SetAnimation(state));
                StartCoroutine(SetAnimation(state));
                break;
            case CreatureState.Eat:
                StopCoroutine(SetAnimation(state));
                StartCoroutine(SetAnimation(state));
                break;
            case CreatureState.dead:
                CreatureDead();
                break;
            case CreatureState.evolve:
                CreatureEvolve();
                break;
        }
    }

    WaitForSeconds wait = new WaitForSeconds(2f);
    IEnumerator SetAnimation(CreatureState state){
        creature.AnimationState.SetAnimation(0, state.ToString(), true).TimeScale = 1f;
        yield return wait;
        creature.AnimationState.SetAnimation(0, CreatureState.stand.ToString(), true).TimeScale = 1f;
    }
    void RegistTickAction()
    {
        
    }
    
    void CreatureDead()
    {
        LoadCreature(true, 0);
        ComponentUtility.Log("DEAD");
    }

    public void CreatureEvolve(int newIndex = -1)
    {
        if(dataManager.PlayerInfo.isDead) return;
        
        if(newIndex == -1)
            newIndex = GetNewEvolveIndex();
        
        dataManager.PlayerInfo.SetCreatureInitTime();
        dataManager.PlayerInfo.creatureIndex = newIndex;
        dataManager.PlayerInfo.SetCreature(newIndex, 1);
        dataManager.PlayerInfo.isDead = false;
        dataManager.PlayerInfo.canEveolve = false;

        LoadCreature(false, newIndex);
        ComponentUtility.Log("EVOLVE");
    }

    int GetNewEvolveIndex(){
        int newIndex = -1;

        int whileBreaker = 0;
        while ((newIndex = UnityEngine.Random
                .Range(0, dataManager.Creature.skeletonDataAssetList.Count)) 
                != dataManager.PlayerInfo.creatureIndex)
            if (whileBreaker++ > 100) break;
        return newIndex;
    }
}
