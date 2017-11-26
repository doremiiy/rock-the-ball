using UnityEngine;
using UnityEngine.Networking;


public class Ball : NetworkBehaviour {

	private Rigidbody rb;
	public Vector3 testForce;
    public float maxSpeed = 1000f;
    public GameManager gameManager;

    void Start () {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Speed cap
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
}
