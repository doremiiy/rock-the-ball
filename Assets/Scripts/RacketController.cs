using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketController : MonoBehaviour {

    private GameObject player;

    void Start()
    {
        player = transform.parent.gameObject;
    }

	void OnTriggerEnter (Collider collider) {
        player.GetComponent<PlayerController>().ballHit(this.gameObject, collider);
	}
}
