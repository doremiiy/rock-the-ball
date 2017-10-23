using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject redPlayerBallSpawn;
    public GameObject bluePlayerBallSpawn;
    public UIScores uiScores;
    public GameObject ball;
    private Dictionary<Utility.Team, int> scores;
    private Dictionary<Utility.Team, GameObject> ballSpawnPoints;
 

    private void Start()
    {
        scores = new Dictionary<Utility.Team, int>
        {
            { Utility.Team.blue, 0 },
            { Utility.Team.red, 0 }
        };
        ballSpawnPoints = new Dictionary<Utility.Team, GameObject>
        {
            { Utility.Team.blue, bluePlayerBallSpawn },
            { Utility.Team.red, redPlayerBallSpawn }
        };
    }

    public void IncreasePlayerScore (Utility.Team team){
        int oldScore = scores[team];
        scores[team] = ++oldScore;
        uiScores.UpdateScores();
        if (oldScore == 15)
        {
            uiScores.DisplayWinText(team);
            // TODO add the end of the game
        } else
        {
            switch (team)
            {
                case Utility.Team.blue:
                    Instantiate(ball, ballSpawnPoints[Utility.Team.red].transform.position, Quaternion.identity);
                    break;
                case Utility.Team.red:
                    Instantiate(ball, ballSpawnPoints[Utility.Team.blue].transform.position, Quaternion.identity);
                    break;
                default:
                    Debug.Log("Unrecognized Team");
                    break;
            }
        }
    }

    public int GetPlayerScore(Utility.Team team){
        return scores[team];
    }
}
