using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private Dictionary<Utility.Team, int> scores;
    public UIScores uiScores;
    public GameObject ball;

    private void Start()
    {
        scores.Add(Utility.Team.blue, 0);
        scores.Add(Utility.Team.red, 0);
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
            // TODO add the process to start a new point
            Instantiate(ball);
        }
    }

    public int GetPlayerScore(Utility.Team team){
        return scores[team];
    }
}
