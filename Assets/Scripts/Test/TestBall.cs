using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBall : MonoBehaviour {

	private Rigidbody rb;
	public Vector3 force;
    public float maxSpeed;
    public GameManager gameManager;

    private bool isServed = false;
    private Utility.Team servingPlayer;

    void Start () {
		rb = GetComponent<Rigidbody>();
		rb.AddForce (force);
	}

    void FixedUpdate()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {   
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
