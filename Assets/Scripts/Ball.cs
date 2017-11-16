using UnityEngine;
using UnityEngine.Networking;


public class Ball : NetworkBehaviour {

	private Rigidbody rb;
	public Vector3 testForce;
    public float maxSpeed;
    public GameManager gameManager;

    private bool isServed = false;
    private Utility.Team servingPlayer;

    // Must be executed everywhere
    void Start () {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody>();
        //rb.AddForce(testForce);
        if (maxSpeed == 0)
        {
            maxSpeed = 10000f;
        }
        isServed = true;
    }

    //public override void OnStartClient()
    //{
    //    base.OnStartClient();
    //    gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    //}

    //public override void OnStartServer()
    //{
    //    base.OnStartServer();
    //    gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    //}

    void FixedUpdate()
    {
        // Speed cap
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Server only for collision detection
        if (!isServer)
        {
            return;
        }

        if (isServed)
        {
            // Check to see if the service is good
            if (other.CompareTag("ServiceZone") && other.GetComponent<ServiceZone>().IsValid)
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
        // Check for a potential goal
        } else if (other.CompareTag("Goal") && other.gameObject.GetComponent<Goal>().isActive)
        {
            gameManager.IncreasePlayerScore(Utility.Opp(other.gameObject.GetComponent<Goal>().team));
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
