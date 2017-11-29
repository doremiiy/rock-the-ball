using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UIManager : NetworkBehaviour {

    private UIScore[] uiScores;
    [SyncVar (hook ="OnChangeTeamScoreTrigger")]
    private Utility.Team teamScoreTrigger;

    public Utility.Team TeamScoreTrigger
    {
        get
        {
            return teamScoreTrigger;
        }

        set
        {
            teamScoreTrigger = value;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    private void Start()
    {
        GameObject[] racketControllers = GameObject.FindGameObjectsWithTag("Racket");
        int index = 0;
        foreach (GameObject racketController in racketControllers)
        {
            uiScores[index++] = racketController.GetComponent<UIScore>();
        }
    }

    public void IncrementScore(Utility.Team team)
    {
        foreach(UIScore uiScore in uiScores)
        {
            uiScore.IncrementScore(team);
        }
    }

    private void OnChangeTeamScoreTrigger(Utility.Team team)
    {
        IncrementScore(team);
    }
}
