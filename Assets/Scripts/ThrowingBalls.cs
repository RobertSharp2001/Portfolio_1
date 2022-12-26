using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ThrowingBalls : MonoBehaviour{
    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject objectToThrow;

    [Header("Settings")]
    public int balls;
    public float throwCooldown;

    [Header("Throwing")]
    public float throwForce;
    public float throwUpwardForce;

    bool readyToThrow;

    private void Start() {
        readyToThrow = true;

        if (cam == null){
            Camera mainCamera = Camera.main;
            if (mainCamera != null) { cam = mainCamera.gameObject.transform; }
        }
    }

    private void Update(){
        
        if (Input.GetMouseButtonDown(0)){
            checkForBarrel();
        } 
        
        if(Input.GetMouseButtonDown(0) && readyToThrow && balls > 0){
            Throw();
        }
    }

    private void checkForBarrel(){
        Vector3 forceDirection = cam.transform.forward;
        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f) && hit.collider.gameObject != null) {
            Debug.Log(hit.collider.gameObject.name);
            
            if (hit.collider.gameObject.tag == "Barrel") {
                GameManager.givePlayerBalls();
                //stop the player from immediately using balls
                readyToThrow = false;
                Invoke(nameof(ResetThrow), throwCooldown);
            }
        }
    }

    private void Throw(){
        readyToThrow = false;

        // instantiate object to throw
        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);

        // get rigidbody component
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        // calculate direction
        Vector3 forceDirection = cam.transform.forward;

        //Raycast from the ball
        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, 500f)){forceDirection = (hit.point - attackPoint.position).normalized;}
        // add force
        Vector3 forceToAdd = forceDirection * throwForce + transform.up * throwUpwardForce;
        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
        
        //Take one of the player's balls
        balls--;

        // stop the player from thrwoing again immediately
        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow(){
        readyToThrow = true;
    }
}