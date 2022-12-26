using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour{
    private void OnTriggerEnter(Collider other){
        //Destory any balls that fall off
        if (other.CompareTag("Projectile")) {
            Destroy(other.gameObject);
        };
        //If it's not a player ignore it
        if (!other.CompareTag("Player")) return;
        //move the player to their respawn point
        other.transform.position = GameManager.respawnPoint.position;
    }
}
