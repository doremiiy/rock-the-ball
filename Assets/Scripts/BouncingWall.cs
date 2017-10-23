using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingWall : MonoBehaviour {

    private Vector3 wallNormal = new Vector3();
    public float BouncingMultiplier;


    private void Start()
    {
        wallNormal = transform.up;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TEST");
        if (other.CompareTag("Ball"))
        {
            Rigidbody ballRigidbody = other.GetComponent<Rigidbody>();
            Vector3 ballSpeed = ballRigidbody.velocity;
            ballSpeed = Vector3.Project(ballSpeed, wallNormal);
            Vector3 force = -ballSpeed * BouncingMultiplier;
            ballRigidbody.AddForce(force, ForceMode.Impulse);
        }
    }
}
