using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetToggle : MonoBehaviour{
    // Start is called before the first frame update
    public Material SwitchMat;
    public bool triggered;

    private void OnTriggerEnter(Collider other){
        if (!other.CompareTag("Projectile")) return;
        triggered = true;
        gameObject.GetComponent<MeshRenderer>().material = SwitchMat;

        GameManager.updateTarget();
    }
}
