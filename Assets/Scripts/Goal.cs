using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

    public GameManager gameManager;
    public Utility.Team team;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Debug.Log("Ball entered the Goal");
            gameManager.IncreasePlayerScore(team);
            Destroy(other.gameObject);
        }
    }
}
