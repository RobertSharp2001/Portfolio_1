using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour{
    //sound
    public float speed;


    private void FixedUpdate(){
        //make the coin spin
        transform.Rotate(transform.localRotation.x + speed,0,0);
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        //add toi the player's coin value
        GameManager.givePlayerCoin();

        this.gameObject.SetActive(false);
    }
}
