using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class GameManager : NetworkBehaviour
{

    public GameObject redPlayerBallSpawn;
    public GameObject bluePlayerBallSpawn;
    public GameObject[] redPlayerServiceZones;
    public GameObject[] bluePlayerServiceZones;
    public UIScores uiScores;
    public GameObject ballPrefab;
    public GameObject ball;


    private Dictionary<Utility.Team, int> scores;

    private Dictionary<Utility.Team, GameObject> ballSpawnPoints;
    private Dictionary<Utility.Team, GameObject[]> serviceZones;
    private ServiceZone currentServiceZone;
  
    [SyncVar]
    private bool triggerGameWin;
    [SyncVar(hook = "OnChangeTriggerPointWin")]
    private bool triggerPointWin;
    [SyncVar]
    private Utility.Team lastPointWinner;
 

    // Clients and server
    private void Start()
    {
        // TODO factorize these tree dictionnary in one
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
        serviceZones = new Dictionary<Utility.Team, GameObject[]>
        {
            { Utility.Team.blue, bluePlayerServiceZones},
            { Utility.Team.red, redPlayerServiceZones}
        };
    }

    public override void OnStartServer() {
        SpawnBall();
    }

    // Get the local Ball
    public override void OnStartClient()
    {
        base.OnStartClient();
        ball = GameObject.FindGameObjectWithTag("Ball");
    }

    // TEST CODE to check the ball movement on the cients
    private void FixedUpdate()
    {
        //ball.GetComponent<Rigidbody>().AddForce(Vector3.forward * Time.fixedDeltaTime * 3f, ForceMode.Impulse);
    }

    public void IncreasePlayerScore(Utility.Team team)
    {
        lastPointWinner = team;
        scores[team]++;
        uiScores.UpdateScores();
        if (scores[team] == Utility.winningScore)
        {
            uiScores.DisplayWinText(team);
            triggerGameWin = !triggerGameWin;
            // TODO End of the game
        }
        else
        {
            StartNewPoint(team);
            triggerPointWin = !triggerPointWin;
            // TODO point's ending
        }
    }

    private void StartNewPoint(Utility.Team wonLastPoint)
    {
        // A new ball is instantiated in front of the player who lost the last point
        var newBall = (GameObject)Instantiate(ballPrefab, ballSpawnPoints[Utility.Opp(wonLastPoint)].transform.position, Quaternion.identity);
        NetworkServer.Spawn(newBall);
        int rand = UnityEngine.Random.Range(0, serviceZones[wonLastPoint].Length);
        currentServiceZone = serviceZones[wonLastPoint][rand].GetComponent<ServiceZone>();
        currentServiceZone.SetIsValid(true);
    }

    public void ResetServiceZone()
    {
        currentServiceZone.SetIsValid(false);
    }

    public int GetPlayerScore(Utility.Team team){
        return scores[team];
    }

    public void RelocateBall(Vector3 newPosition)
    {
        ball.transform.position = newPosition;
    }

    private void SpawnBall()
    {
        ball = (GameObject)Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(ball);
    }

   // USELESS replace by a syncVar
    //private void OnScoresChange(Dictionary<Utility.Team, int> newScores)
    //{
        //foreach (Utility.Team team in Enum.GetValues(typeof(Utility.Team)))
        //{
        //    Debug.Log(team);
        //    if (newScores[team] > scores[team])
        //    {
        //        Debug.Log("Team :" + team + " won the last point");
        //        lastPointWinner = team;
        //        break;
        //    }
        //}
    //}

    private void OnChangeTriggerPointWin(bool newVal)
    {
        // TODO launch the sequence for a new point
    }

    private void OnChangeLastPointWinner(Utility.Team newTeam)
    {
        scores[newTeam]++;
    }
}
