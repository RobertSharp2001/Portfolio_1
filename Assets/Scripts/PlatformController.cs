using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour{

    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private float moveSpeed = 5f;
    private int currentWaypoint;

    // Start is called before the first frame update
    private void Start(){
        if (waypoints.Count <= 0) return;
        currentWaypoint = 0;
    }

    // Update is called once per frame
    private void FixedUpdate(){
        HandleMovement();
    }

    private void HandleMovement() {

        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypoint].transform.position,(moveSpeed * Time.deltaTime));

        if (Vector3.Distance(waypoints[currentWaypoint].transform.position, transform.position) <= 0) {
            currentWaypoint++;
        }

        if (currentWaypoint != waypoints.Count) return;
        waypoints.Reverse();
        currentWaypoint = 0;
    }
    private void OnTriggerEnter(Collider other){
        if (!other.CompareTag("Player")) return;
        other.transform.parent = transform;
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Player")) return;
        other.transform.parent = null;
    }
}
