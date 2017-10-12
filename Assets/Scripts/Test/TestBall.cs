using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBall : MonoBehaviour {

	private Rigidbody rb;
	public Vector3 force;
	void Start () {
		rb = GetComponent<Rigidbody>();
		rb.AddForce (force);
	}

	void FixedUpdate () {
		
	}
}
