using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Consts;

public class ObjectManager : MonoBehaviour
{
    ActionManager actionManager;
    [SerializeField]
    List<GameObject> objectDirtList = new List<GameObject>();
    [SerializeField]
    List<GameObject> objectPropList = new List<GameObject>();

    Coroutine coroutine = null;

    private void Awake() 
    {
        actionManager = this.GetComponent<ActionManager>();
        RegistTickAction();
        RegistPropAction();
        actionManager.initFlag[nameof(ObjectManager)] = true;
    }

    void RegistTickAction(){
        actionManager.RegistTickAction(Action_ShowDirt);
    }

    void Action_ShowDirt()
    {
        for (int i = 0; i < 3; i++)
            objectDirtList[i].gameObject.SetActive(
                !actionManager.CheckActionCondition(ConditionCheckType.dirt, GameLoop.dirtDegree[i]));
    }
    
    void RegistPropAction(){
        actionManager.RegistPropAction(ShowProp);
    }

    void ShowProp(string name) 
    {
        foreach(GameObject prop in objectPropList) 
                prop.SetActive(false);
        if(coroutine != null)
            StopCoroutine(coroutine);
        foreach(GameObject prop in objectPropList) 
            if(prop.name.ToLower().Contains(name.ToLower()))
                coroutine = StartCoroutine(ShowPropCoroutine(prop));
    }

    IEnumerator ShowPropCoroutine(GameObject prop) 
    {
        Debug.Log(prop.name);
        prop.SetActive(true);
        yield return new WaitForSeconds(GameLoop.animationTime);
        prop.SetActive(false);
        coroutine = null;
    }
}
