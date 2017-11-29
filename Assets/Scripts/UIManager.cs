using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UIManager : NetworkBehaviour {

    public List<UIScore> uiScores;
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

    public void IncrementScore(Utility.Team team)
    {
        foreach(UIScore uiScore in uiScores)
        {
            uiScore.IncrementScore(team);
        }
    }

    public void AddPlayerUI(UIScore uiScore)
    {
        uiScores.Add(uiScore);
    }

    private void OnChangeTeamScoreTrigger(Utility.Team team)
    {
        IncrementScore(team);
    }
}
