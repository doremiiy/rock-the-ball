using UnityEngine;
using UnityEngine.Networking;


public class Ball : NetworkBehaviour {

	private Rigidbody rb;
	public Vector3 force;
    public float maxSpeed;
    public GameManager gameManager;

    private bool isServed = false;
    private Utility.Team servingPlayer;

    // Must be executed everywhere
    void Start () {
		rb = GetComponent<Rigidbody>();
        rb.AddForce(force);
        if (maxSpeed == 0)
        {
            maxSpeed = 10000f;
        }
    }

    void FixedUpdate()
    {
        // Can be executed everywhere, on clients and server. No need to use the network here
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // This code should be executed only on the server, no need for redundancy, the server must have authority
        if (!isServer)
        {
            return;
        }

        if (isServed)
        {
            // Check to see if the service is good
            if (other.CompareTag("ServiceZone") && other.GetComponent<ServiceZone>().GetIsValid())
            {
                Debug.Log("Service in");
                isServed = false;
            }
            else
            {
                Debug.Log("Service out");
                gameManager.IncreasePlayerScore(Utility.Opp(servingPlayer));
            }
            gameManager.ResetServiceZone();
        } else if (other.CompareTag("Ball"))
        {
            Debug.Log("Ball entered the Goal");
            gameManager.IncreasePlayerScore(other.gameObject.GetComponent<Goal>().team);
            // Destroy on every client
            Network.Destroy(other.gameObject);
        }
    }

    public bool GetIsServed()
    {
        return isServed;
    }

    public void SetIsServed(bool newVal)
    {
        isServed = newVal;
    }

    public Utility.Team GetServingPlayer()
    {
        return servingPlayer;
    }

    public void SetServingPlayer(Utility.Team newVal)
    {
        servingPlayer = newVal;
    }
}
