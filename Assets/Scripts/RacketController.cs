using System.Collections;
using UnityEngine;

public class RacketController : MonoBehaviour
{

    public PlayerController playerController;
    public SoundManager soundManager;
    public Utility.Hand hand;
    public bool shouldVibrate;
    private bool isActive;

    public bool IsActive
    {
        get
        {
            return isActive;
        }

        set
        {
            isActive = value;
            if (!IsActive)
            {
                StartCoroutine(ReactivateCollider(0.1f));
            }
        }
    }

    void Start()
    {
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
    }


    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Ball") && IsActive)
        {
            IsActive = false;
            Debug.Log("Racket Controller : collision detected");
            playerController.BallHit(hand);
            soundManager.PlaySound("RacketHit");
            if (shouldVibrate)
            {
                //GetComponent<OVRVibration>().VibrateController(Utility.viveControllerNode, 5, 500);
            }
        }
    }

    IEnumerator ReactivateCollider(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        IsActive = true;
    }
}
