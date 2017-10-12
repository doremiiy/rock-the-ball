using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScores : MonoBehaviour {

    public Text bluePlayerScore;
    public Text redPlayerScore;
    public Text winText;
    public GameManager gameManager;

	void Start () {
        bluePlayerScore.text = "0";
        redPlayerScore.text = "0";
        winText.text = "";
    }
	
    public void UpdateScores() {
        bluePlayerScore.text = gameManager.GetPlayerScore(Utility.Team.blue).ToString();
        redPlayerScore.text = gameManager.GetPlayerScore(Utility.Team.red).ToString();
    }

    public void DisplayWinText(Utility.Team WinningTeam)
    {
        if (WinningTeam == Utility.Team.blue)
        {
            winText.text = "Blue Player wins the game";
        } else if (WinningTeam == Utility.Team.red)
        {
            winText.text = "Red Player wins the game";
        }
    }
}
