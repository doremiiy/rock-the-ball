using UnityEngine;

public class Goal : MonoBehaviour {

    // TODO replace public variables by getters and setters

    public Utility.Team team;
    public bool isActive;
    public GameManager gameManager;
    public ServiceManager serviceManager;
    public SoundManager soundManager;

    public GameObject plasmaExplosion;

    private void Start()
    {
        if (GameState.training)
        {
            isActive = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            if (serviceManager.IsServed)
            {
                serviceManager.HandleService(false);
            } else if (isActive)
            // TODO handle this processing on the server only
            {
                gameManager.IncreasePlayerScore(Utility.Opp(team));
                soundManager.PlaySound("Goal");
                Instantiate(plasmaExplosion, other.transform.position, Quaternion.identity);

                if (GameState.training &&  
                    (gameManager.TrainingStep == Utility.TrainingStep.GOAL ||
                    gameManager.TrainingStep == Utility.TrainingStep.SERVICE))
                {
                    gameManager.AccessNextTrainingStep();
                }
            }
        }
    }

    public void Explode(Vector3 position)
    {
        Instantiate(plasmaExplosion, position, Quaternion.identity);
    }
}
