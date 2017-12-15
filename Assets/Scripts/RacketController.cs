using UnityEngine;
using UnityEngine.VR;

public class RacketController : MonoBehaviour {

    public PlayerController playerController;
    public SoundManager soundManager;
    public bool shouldVibrate;

    void Start()
    {
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    }

    
	void OnTriggerEnter (Collider collider) {
        if (collider.CompareTag("Ball"))
        {
            Debug.Log("Racket Controller : collision detected");
            playerController.BallHit();
            soundManager.PlaySound("RacketHit");
            if (shouldVibrate)
            {
                GetComponent<OVRVibration>().VibrateController(Utility.viveControllerNode, 5, 500);
            }
        }
	}
}
