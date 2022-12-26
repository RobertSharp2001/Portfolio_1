using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This movement script was inspired by, and attempts to copy the movement of the game Quake 3*/


struct UserInput{
    public float forward;
    public float right;
}

[RequireComponent(typeof(CharacterController))]
public class PlayerScript : MonoBehaviour{
    // Start is called before the first frame update

    [Header("View control")]
    public Transform playerView;     // Camera
    public float StartViewDir;
    public float playerViewYOffset = 0.6f; // The height at which the camera is bound to
    public float xMouseSensitivity = 30.0f;
    public float yMouseSensitivity = 30.0f;
    // Camera rotations
    private float camX = 0.0f;
    private float camY = 0.0f;
    private CharacterController controller;
    //
    [Header("Constants")]
    public float gravity = 20.0f;
    public float friction = 6; //Ground friction
    private float playerFriction = 0.0f; // display real time fricton values
    public float moveSpeed = 7.0f;// Ground move speed
    public static GameObject respawnPoint;

    [Header("Variable Values")]
    public float runAcceleration = 14.0f;         // Ground accel
    public float runDeacceleration = 10.0f;       // Deacceleration that occurs when running on the ground
    public float airAcceleration = 2.0f;          // Air accel
    public float airDecceleration = 2.0f;         // Deacceleration experienced when ooposite strafing
    public float airControl = 0.3f;               // How precise air control is
    public float sideStrafeAcceleration = 50.0f;  // How fast acceleration occurs to get up to sideStrafeSpeed when
    public float sideStrafeSpeed = 1.0f;          // What the max speed to generate when side strafing

    [Header("Jumping")]
    public float jumpSpeed = 8.0f;                // The speed at which the character's up axis gains when hitting jump
    public bool holdToBhop = false;
    private bool wishJump = false; //Allows Queuing jumps

    [Header("GUI Stuff")]
    public GUIStyle style;

    [Header("FPS")]
    public float fpsDisplayRate = 4.0f; // 4 updates per sec
    private int frameCount = 0;
    private float dt = 0.0f;
    private float fps = 0.0f;

    [Header("Vectors")]
    private Vector3 moveDirectionNorm = Vector3.zero;
    private Vector3 playerVelocity = Vector3.zero;
    private float playerTopVelocity = 0.0f;

    // Player input, stores wish commands that the player asks for (Forward, back, jump, etc)
    private UserInput userInput;

    private void Start() {
        // Hide the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //Make sure the camera is enabled
        if (playerView == null){
            Camera mainCamera = Camera.main;
            if (mainCamera != null) { playerView = mainCamera.gameObject.transform; }
        }

        // Make sure the camera is inside the capsule collider
        playerView.position = new Vector3(
            transform.position.x,
            transform.position.y + playerViewYOffset,
            transform.position.z);

        controller = GetComponent<CharacterController>();
        controller.transform.Rotate(0, StartViewDir, 0);
    }

    private void SetMovementDir(){
        userInput.forward = Input.GetAxisRaw("Vertical");
        userInput.right = Input.GetAxisRaw("Horizontal");
    }

    // Update is called once per frame
    private void Update() {
        // Calculate FPS
        frameCount++;
        dt += Time.deltaTime;

        if (dt > 1.0 / fpsDisplayRate){
            fps = Mathf.Round(frameCount / dt);
            frameCount = 0;
            dt -= 1.0f / fpsDisplayRate;
        }
        /* Ensure that the cursor is locked into the screen */
        if (Cursor.lockState != CursorLockMode.Locked){
            if (Input.GetButtonDown("Fire1"))
                Cursor.lockState = CursorLockMode.Locked;
        }

        // Camera rotation stuff, mouse controls
        camX -= Input.GetAxisRaw("Mouse Y") * xMouseSensitivity * 0.02f;
        camY += Input.GetAxisRaw("Mouse X") * yMouseSensitivity * 0.02f;

        // Clamp the X rotation
        if (camX < -90)
            camX = -90;
        else if (camX > 90)
            camX = 90;

        // Rotate the collider
        this.transform.rotation = Quaternion.Euler(0, camY, 0);
        // Rotate the camera
        playerView.rotation = Quaternion.Euler(camX, camY, 0);

        //Debug.Log(controller.isGrounded);

        QueueJump();
        if (controller.isGrounded) {
            GroundMove();
        }
        else if (!controller.isGrounded) {
            AirMove();
        }

        // Move the player
        controller.Move(playerVelocity * Time.deltaTime);

        //Calculate max velocity
        Vector3 udp = playerVelocity;
        udp.y = 0.0f;
        if (udp.magnitude > playerTopVelocity) { playerTopVelocity = udp.magnitude; }

        //Move the camera after the player has been move.
        // Set the camera's position to the transform
        CamUpdate();
    }

    #region movement
    //Movement on the ground
    private void GroundMove(){
        Vector3 wishdir;

        // Dont apply friction if the player is queueing jump
        if (!wishJump)
            ApplyFriction(1.0f);
        else
            ApplyFriction(0);

        SetMovementDir();

        //Direction the player wants to move
        wishdir = new Vector3(userInput.right, 0, userInput.forward);
        wishdir = transform.TransformDirection(wishdir);
        wishdir.Normalize();
        moveDirectionNorm = wishdir;

        var wishspeed = wishdir.magnitude;
        wishspeed *= moveSpeed;

        ApplyAcceleration(wishdir, wishspeed, runAcceleration);

        // Reset the gravity velocity
        playerVelocity.y = -gravity * Time.deltaTime;

        if (wishJump) {
            playerVelocity.y = jumpSpeed;
            wishJump = false;
        }
    }

    private void QueueJump(){
        if (holdToBhop){
            wishJump = Input.GetButton("Jump");
            return;
        }

        if (Input.GetButtonDown("Jump") && !wishJump) { wishJump = true; }
        if (Input.GetButtonUp("Jump")) { wishJump = false; }

    }

    private void AirMove() {
        Vector3 wishdir;
        float wishvel = airAcceleration;
        float accel;

        SetMovementDir();
        //Direction the player wants to move
        wishdir = new Vector3(userInput.right, 0, userInput.forward);
        wishdir = transform.TransformDirection(wishdir);

        float wishspeed = wishdir.magnitude;
        wishspeed *= moveSpeed;

        wishdir.Normalize();
        moveDirectionNorm = wishdir;

        // Aircontrol
        float wishspeed2 = wishspeed;
        if (Vector3.Dot(playerVelocity, wishdir) < 0) {
            accel = airDecceleration;
        } else{
            accel = airAcceleration;
        }
        // If the player is strafing left or right
        if (userInput.forward == 0 && userInput.right != 0){
            if (wishspeed > sideStrafeSpeed) { wishspeed = sideStrafeSpeed; }        
            accel = sideStrafeAcceleration;
        }

        ApplyAcceleration(wishdir, wishspeed, accel);
        if (airControl > 0) { AirControl(wishdir, wishspeed2); }

        // Apply gravity
        playerVelocity.y -= gravity * Time.deltaTime;
    }

    private void AirControl(Vector3 wishdir, float wishspeed){
        float zspeed,speed,dot,k;

        // prevent control of movement if not moving forward or backward
        if (Mathf.Abs(userInput.forward) < 0.001 || Mathf.Abs(wishspeed) < 0.001) {
            return;
        }

        zspeed = playerVelocity.y;
        playerVelocity.y = 0;

        //Normalize movement
        speed = playerVelocity.magnitude;
        playerVelocity.Normalize();

        dot = Vector3.Dot(playerVelocity, wishdir);
        //k is constant
        k = 32;
        k *= airControl * dot * dot * Time.deltaTime;

        // Change direction while slowing down
        if (dot > 0){
            playerVelocity.x = playerVelocity.x * speed + wishdir.x * k;
            playerVelocity.y = playerVelocity.y * speed + wishdir.y * k;
            playerVelocity.z = playerVelocity.z * speed + wishdir.z * k;

            playerVelocity.Normalize();
            moveDirectionNorm = playerVelocity;
        }

        //apply changes
        playerVelocity.x *= speed;
        playerVelocity.y = zspeed; 
        playerVelocity.z *= speed;
    }

    #endregion

    #region forces
    private void ApplyFriction(float t) {
        Vector3 vec = playerVelocity; // Equivalent to: VectorCopy();
        float speed, newspeed, control, drop;

        //Default values
        vec.y = 0.0f;
        speed = vec.magnitude;
        drop = 0.0f;

        // if the player is on the ground then apply
        if (controller.isGrounded) {
            control = speed < runDeacceleration ? runDeacceleration : speed;
            drop = control * friction * Time.deltaTime * t;
        }

        newspeed = speed - drop;
        playerFriction = newspeed;
        //make sure the value isn't negative
        if (newspeed < 0)
            newspeed = 0;
        if (speed > 0)
            newspeed /= speed;

        //apply the new speed
        playerVelocity.x *= newspeed;
        playerVelocity.z *= newspeed;
    }

    private void ApplyAcceleration(Vector3 wishdir, float wishspeed, float accel){
        float addspeed, accelspeed,currentspeed;

        currentspeed = Vector3.Dot(playerVelocity, wishdir);
        //add the speed together
        addspeed = wishspeed - currentspeed;
        //make sure it isn't negative
        if (addspeed <= 0)
            return;
        //apply the acceleration over time
        accelspeed = accel * Time.deltaTime * wishspeed;
        if (accelspeed > addspeed)
            accelspeed = addspeed;

        //aply the change in speed
        playerVelocity.x += accelspeed * wishdir.x;
        playerVelocity.z += accelspeed * wishdir.z;
    }
    #endregion

    public void CamUpdate(){
        playerView.position = new Vector3(
            transform.position.x,
            transform.position.y + playerViewYOffset,
            transform.position.z);
    }

    private void OnGUI(){
        GUI.Label(new Rect(0, 0, 400, 100), "FPS: " + fps, style);
        var ups = controller.velocity;
        ups.y = 0;
        GUI.Label(new Rect(0, 15, 400, 100), "Speed: " + Mathf.Round(ups.magnitude * 100) / 100 + "ups", style);
        GUI.Label(new Rect(0, 30, 400, 100), "Top Speed: " + Mathf.Round(playerTopVelocity * 100) / 100 + "ups", style);
    }


}
