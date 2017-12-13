using UnityEngine;
using UnityEngine.VR;

public class RacketController : MonoBehaviour {

    private GameObject player;
    private PlayerController playerController;
    public SoundManager soundManager;

    void Start()
    {
        player = transform.parent.gameObject;
        playerController = player.GetComponent<PlayerController>();
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    }

    
	void OnTriggerEnter (Collider collider) {
        if (collider.CompareTag("Ball"))
        {
            Debug.Log("Racket Controller : collision detected");
            playerController.BallHit();
            soundManager.PlaySound("RacketHit");
            GetComponent<OVRVibration>().VibrateController(Utility.viveControllerNode, 5, 500);
        }
	}
}
