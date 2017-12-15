using UnityEngine;

public class RacketController : MonoBehaviour {

    public PlayerController playerController;
    public SoundManager soundManager;
    public Utility.Hand hand;
    public bool shouldVibrate;

    void Start()
    {
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    }

    
	void OnTriggerEnter (Collider collider) {
        if (collider.CompareTag("Ball"))
        {
            Debug.Log("Racket Controller : collision detected");
            playerController.BallHit(hand);
            soundManager.PlaySound("RacketHit");
            if (shouldVibrate)
            {
                GetComponent<OVRVibration>().VibrateController(Utility.viveControllerNode, 5, 500);
            }
        }
	}
}
