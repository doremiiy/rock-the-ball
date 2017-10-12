using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBall : MonoBehaviour {

	void Start () {
		Rigidbody rb = GetComponent<Rigidbody>();
		rb.AddForce (new Vector3 (100, 0, 0));
	}
}
