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

    private void Awake() {
        actionManager = this.GetComponent<ActionManager>();

        actionManager.initFlag[nameof(ObjectManager)] = true;
    }

    
    void ShowProp(int index ) {
        foreach(GameObject prop in objectPropList) 
                prop.SetActive(false);
        StopCoroutine(ShowPropCoroutine(objectPropList[index]));
        StartCoroutine(ShowPropCoroutine(objectPropList[index]));
    }
    IEnumerator ShowPropCoroutine(GameObject prop) {
        prop.SetActive(true);
        yield return new WaitForSeconds(GameLoop.animationTime);
        prop.SetActive(false);
    }

}
