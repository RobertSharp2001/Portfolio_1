using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorObjective : MonoBehaviour{
    // Start is called before the first frame update
    public TargetToggle[] targets;

    public GameObject[] objToRmv;


    public void Update() {
        checkTargets();
    }

    public void checkTargets(){
        for (int i = 0; i < targets.Length; i++){
            if (targets[i].triggered == false) return;
        }

        for (int i = 0; i < objToRmv.Length; i++){
            objToRmv[i].SetActive(false);
        }

        gameObject.SetActive(false);
    }
}
