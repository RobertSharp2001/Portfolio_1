using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour{
    //player's start
    public static Transform respawnPoint;
    public Transform initStartPoint;
    //player's coins
    public static int coins;

    public Text coinsText;
    public Text ballsText;
    [Header("Audio")]
    public static AudioSource barrelSounds;
    public static AudioSource coinSounds;
    public static AudioSource checkpointSounds;
    public static AudioSource targetSounds;
    public static GameObject player;

    public void Start(){
        respawnPoint = initStartPoint;
        player = GameObject.FindGameObjectWithTag("Player");

        //Audio
        barrelSounds = GameObject.FindGameObjectWithTag("Barrel Audio").GetComponent<AudioSource>();
        coinSounds = GameObject.FindGameObjectWithTag("Coin Audio").GetComponent<AudioSource>();
        checkpointSounds = GameObject.FindGameObjectWithTag("Checkpoint Audio").GetComponent<AudioSource>();
        targetSounds = GameObject.FindGameObjectWithTag("Target Audio").GetComponent<AudioSource>();
    }

    public void Update(){
        coinsText.text = "x " + coins.ToString();
        ballsText.text = "x " + player.GetComponent<ThrowingBalls>().balls.ToString();

        if (Input.GetKey("escape")){
            Application.Quit();
        }
        //Debug.Log(respawnPoint);
    }

    public static void givePlayerBalls(){
        player.GetComponent<ThrowingBalls>().balls = 3;
        barrelSounds.Play();
    }

    public static void givePlayerCoin(){
        coins++;
        coinSounds.Play();
    }

    public static void updateCheckpoint(Transform newRespawn){
        respawnPoint = newRespawn;
        checkpointSounds.Play();
    }

    public static void updateTarget(){
        targetSounds.Play();
    }

}
