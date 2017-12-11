using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UIManager : NetworkBehaviour {

    public List<UIScore> uiScores;
    [SyncVar (hook ="OnChangeTeamScoreTrigger")]
    private Utility.Team teamScoreTrigger;
    private GameObject mainUI;

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

    public void ShowTrainingText()
    {
        GameObject text = mainUI.transform.Find("MainText").gameObject;
        text.GetComponent<Text>().text = Utility.trainingNextStepText;
        text.SetActive(true);
    }



    public void HideMainText()
    {
        GameObject text = mainUI.transform.Find("MainText").gameObject;
        text.SetActive(false);
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

    public void SetUpMainUI()
    {
        mainUI = GameObject.FindGameObjectWithTag("MainUI");
        mainUI.transform.Rotate(GameState.mainUIRotation);
    }
}
