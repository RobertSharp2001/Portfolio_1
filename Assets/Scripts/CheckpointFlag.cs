using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckpointFlag : MonoBehaviour{
    // Start is called before the first frame update

    public Transform newCheckpoint;
    public GameObject CheckpointText;
    // amkes sure a checkpoint cannot be triggered twice
    public bool triggered = false;

    public void Start(){
        CheckpointText = GameObject.FindGameObjectWithTag("Checkpoint Text");
    }


    private void OnTriggerEnter(Collider other){
        if (!other.CompareTag("Player") || triggered) return;
        //update the player's respawn point
        GameManager.updateCheckpoint(newCheckpoint);
        //Make the text visible
        CheckpointText.GetComponent<Text>().text = "Checkpoint reached!";
        //Turn the text off after 3 seconds
        Invoke(nameof(DisableCheckpointText),3f);

        //Prevent the checkpoint from being obtained again
        triggered = true;
    }

    private void DisableCheckpointText(){
        CheckpointText.GetComponent<Text>().text = "";
    }
}
