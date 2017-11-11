using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketController : MonoBehaviour {

    private GameObject player;
    private PlayerController playerController;

    void Start()
    {
        player = transform.parent.gameObject;
        playerController = player.GetComponent<PlayerController>();
    }

	void OnTriggerEnter (Collider collider) {
        playerController.BallHit(this.gameObject, collider);
	}
}
