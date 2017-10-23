using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBall : MonoBehaviour {

	private Rigidbody rb;
	public Vector3 force;
    public float maxSpeed;

    void Start () {
		rb = GetComponent<Rigidbody>();
		rb.AddForce (force);
	}

    void FixedUpdate()
    {
        //Debug.Log(rb.velocity);
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
}
