using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

    public Utility.Team team;
    public bool isActive;
    public GameManager gameManager;
    public ServiceManager serviceManager;
    public SoundManager soundManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            if (serviceManager.IsServed)
            {
                serviceManager.HandleService(false);
            } else if (isActive)
            {
                gameManager.IncreasePlayerScore(Utility.Opp(team));
                soundManager.PlaySound("Goal");
            }
        }
    }
}
