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
        if (collider.CompareTag("Ball"))
        {
            Debug.Log("Racket Controller : collision detected");
            playerController.BallHit(handSide);
        }
	}
}
