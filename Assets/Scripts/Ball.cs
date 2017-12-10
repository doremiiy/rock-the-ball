using UnityEngine;
using UnityEngine.Networking;


public class Ball : NetworkBehaviour {

	private Rigidbody rb;
	public Vector3 testForce;
    public float maxSpeed = 1000f;
    public GameObject ballUI;

    private void Start () {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Speed cap
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    public void SwitchBallUIActivation(bool isEnabled)
    {
        ballUI.SetActive(isEnabled);
    }
}
