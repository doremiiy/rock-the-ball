using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RacketController : MonoBehaviour {

    private GameObject player;
    private PlayerController playerController;
    public Utility.Hand handSide;

    void Start()
    {
        player = transform.parent.gameObject;
        playerController = player.GetComponent<PlayerController>();
    }

    
	void OnTriggerEnter (Collider collider) {
       Debug.Log("Racket Controller : collision detected");
       playerController.CmdBallHit(handSide);
	}
}
