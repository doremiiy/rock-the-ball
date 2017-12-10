using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingWall : MonoBehaviour {

    private Vector3 wallNormal = new Vector3();
    public bool isActive = true;
    public float BouncingMultiplier;
    public ServiceManager serviceManager;
    public SoundManager soundManager;

    private void Start()
    {
        wallNormal = transform.up;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActive && other.CompareTag("Ball"))
        {   
            if (serviceManager.IsServed)
            {
                serviceManager.HandleService(false);
            } else
            {
                Debug.Log("OK");
                Rigidbody ballRigidbody = other.GetComponent<Rigidbody>();
                Vector3 ballSpeed = ballRigidbody.velocity;
                ballSpeed = Vector3.Project(ballSpeed, wallNormal);
                Vector3 force = -ballSpeed * BouncingMultiplier;
                ballRigidbody.AddForce(force, ForceMode.Impulse);
                soundManager.PlaySound("WallHit");
            }
        }
    }
}
