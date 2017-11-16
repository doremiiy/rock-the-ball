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

    private Dictionary<Utility.Team, bool> playersReady;
    private bool isWaitingForPlayers;
    private bool mustStartNewPoint;

    public bool IsWaitingForPlayers
    {
        get
        {
            return isWaitingForPlayers;
        }

        set
        {
            isWaitingForPlayers = value;
        }
    }

    public Dictionary<Utility.Team, bool> PlayersReady
    {
        get
        {
            return playersReady;
        }

        set
        {
            // TODO check if it works
            playersReady = value;
            if (!PlayersReady.ContainsValue(false))
            {
                isWaitingForPlayers = false; 
            } else
            {
                isWaitingForPlayers = true;
            }
        }
    }

    public bool MustStartNewPoint
    {
        get
        {
            return mustStartNewPoint;
        }

        set
        {
            mustStartNewPoint = value;
        }
    }

    // Clients and server
    private void Start()
    {
        // TODO factorize these tree dictionnaries in one
        // TODO initialize the dictionnaries by iterating on tne enum
        scores = new Dictionary<Utility.Team, int>
        {
            { Utility.Team.blue, 0 },
            { Utility.Team.red, 0 }
        };
    }

    private void Update()
    {
        if (!isServer)
        {
            return;
        }

        if (MustStartNewPoint && !IsWaitingForPlayers)
        {
            StartNewPoint();
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

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
        PlayersReady = new Dictionary<Utility.Team, bool>
        {
            { Utility.Team.blue, true},
            { Utility.Team.red, true}
        };

        // pick a random team in the enum to start the match
        Utility.Team randomTeam = Utility.RandomTeam();
        SpawnBall(ballSpawnPoints[randomTeam].transform.position);
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
            triggerPointWin = !triggerPointWin;
        }
    }

    private void StartNewPoint()
    {
        // A new ball is instantiated in front of the player who lost the last point
        var newBall = (GameObject)Instantiate(ballPrefab, ballSpawnPoints[Utility.Opp(lastPointWinner)].transform.position, Quaternion.identity);
        NetworkServer.Spawn(newBall);
        ResetServiceZone();
        currentServiceZone = RandomServiceZone();
        currentServiceZone.IsValid = true;
    }

    public void ResetServiceZone()
    {
        currentServiceZone.IsValid = false;
    }

    private ServiceZone RandomServiceZone()
    {
        int rand = UnityEngine.Random.Range(0, serviceZones[lastPointWinner].Length);
        currentServiceZone = serviceZones[lastPointWinner][rand].GetComponent<ServiceZone>();
        return currentServiceZone;
    }

    public int GetPlayerScore(Utility.Team team){
        return scores[team];
    }

    public void RelocateBall(Vector3 newPosition)
    {
        ball.transform.position = newPosition;
    }

    private void SpawnBall(Vector3 position)
    {
        ball = (GameObject)Instantiate(ballPrefab, position, Quaternion.identity);
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
        // wait for the players to go in their waiting zone
        foreach (Utility.Team team in Enum.GetValues(typeof(Utility.Team)))
        {
            PlayersReady[team] = false;
        }
        IsWaitingForPlayers = true;
    }

    private void OnChangeLastPointWinner(Utility.Team newTeam)
    {
        scores[newTeam]++;
    }
}
