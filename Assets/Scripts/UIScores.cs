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
        bluePlayerScore.text = gameManager.GetPlayerScore(Utility.Team.BLUE).ToString();
        redPlayerScore.text = gameManager.GetPlayerScore(Utility.Team.RED).ToString();
    }

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
}
