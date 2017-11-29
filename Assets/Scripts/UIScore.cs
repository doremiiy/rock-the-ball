using UnityEngine;
using UnityEngine.UI;
using System;

public class UIScore : MonoBehaviour {

    public Text myScore;
    public Text opponentScore;
    public Utility.Team myTeam; 

	void Start () {
        myScore.text = "0";
        opponentScore.text = "0";
        GameObject uiManager = GameObject.FindGameObjectWithTag("UIManager");
        if (uiManager != null)
        {
            uiManager.GetComponent<UIManager>().AddPlayerUI(this);
        } 
    }

    public void IncrementScore(Utility.Team team)
    {
        int newScore;

        if (team == myTeam)
        {
            Int32.TryParse(myScore.text, out newScore);
            newScore++;
            myScore.text = newScore.ToString();
        } else if (team == Utility.Opp(myTeam))
        {
            Int32.TryParse(opponentScore.text, out newScore);
            newScore++;
            opponentScore.text = newScore.ToString();
        }  else
        {
            Debug.Log("UIScore - UpdateScore method - Error, unrecognized team passed as an argument");
        }
    }
}
