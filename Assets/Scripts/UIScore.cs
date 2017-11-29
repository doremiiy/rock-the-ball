using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIScore : MonoBehaviour {

    public Text myScore;
    public Text opponentScore;
    public Text winText;
    public GameManager gameManager;
    public Utility.Team myTeam; 

	void Start () {
        myScore.text = "0";
        opponentScore.text = "0";
        winText.text = "";
    }

    // TODO add a command in the playerController to add the racket in the UIManager

    public void DisplayWinText(Utility.Team WinningTeam)
    {
        if (WinningTeam == Utility.Team.BLUE)
        {
            winText.text = "Blue Player wins the game";
        } else if (WinningTeam == Utility.Team.RED)
        {
            winText.text = "Red Player wins the game";
        }
    }

    public void IncrementScore(Utility.Team team)
    {
        int newScore;
        
        if (team == myTeam)
        {
            Int32.TryParse(myScore.text, out newScore);
            myScore.text = newScore.ToString();
        } else if (team == Utility.Opp(myTeam))
        {
            Int32.TryParse(opponentScore.text, out newScore);
            opponentScore.text = newScore.ToString();
        }  else
        {
            Debug.Log("UIScore - UpdateScore method - Error, unrecognized team passed as an argument");
        }
    }
}
