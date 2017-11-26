using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class GameManager : NetworkBehaviour
{

    public GameObject redPlayerBallSpawn;
    public GameObject bluePlayerBallSpawn;
    public UIScores uiScores;
    public GameObject ballPrefab;
    public GameObject ball;

    private ServiceManager serviceManager;

    private Dictionary<Utility.Team, int> scores;

    private Dictionary<Utility.Team, GameObject> ballSpawnPoints;
    public GameObject[] serviceZones;
    private ServiceZone currentServiceZone;

    [SyncVar(hook = "OnChangeCurrentServiceZoneIndex")]
    private int currentServiceZoneIndex;

    [SyncVar]
    private bool triggerGameWin;
    [SyncVar(hook = "OnChangeTriggerPointWin")]
    private bool triggerPointWin;
    [SyncVar]
    private Utility.Team lastPointWinner;
    [SyncVar]
    private Utility.Team server;

    private Dictionary<Utility.Team, bool> playersReady;
    private bool isWaitingForPlayers;
    private bool mustStartNewPoint;
    [SyncVar (hook ="OnChangeTriggerNewBall")]
    private bool triggerNewBall;

    public Utility.Team startSide;

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
                Debug.Log("All players ready");
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

    public GameObject Ball
    {
        get
        {
            return ball;
        }

        set
        {
            ball = value;
        }
    }

    private void Update()
    {
        if (!isServer)
        {
            return;
        }

        if (MustStartNewPoint && !IsWaitingForPlayers || Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Test ok, starting a new point");
            MustStartNewPoint = false;
            StartNewPoint();
        }
    }

    private void Start()
    {
        scores = new Dictionary<Utility.Team, int>
        {
            { Utility.Team.BLUE, 0 },
            { Utility.Team.RED, 0 }
        };

        serviceManager = GameObject.FindGameObjectWithTag("ServiceManager").GetComponent<ServiceManager>();
        serviceManager.GameManager = this;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        // Only accessed by the server
        PlayersReady = new Dictionary<Utility.Team, bool>
        {
            { Utility.Team.BLUE, true},
            { Utility.Team.RED, true}
        };

        // Only accessed by the server
        ballSpawnPoints = new Dictionary<Utility.Team, GameObject>
        {
            { Utility.Team.BLUE, bluePlayerBallSpawn },
            { Utility.Team.RED, redPlayerBallSpawn }
        };

        server = startSide;
        StartNewPoint();
    }

    // Get the local Ball
    public override void OnStartClient()
    {
        base.OnStartClient();
        Ball = GameObject.FindGameObjectWithTag("Ball");
    }

    public void IncreasePlayerScore(Utility.Team team)
    {
        lastPointWinner = team;
        scores[team]++;
        Network.Destroy(Ball);
        //uiScores.UpdateScores();
        if (scores[team] == Utility.winningScore)
        {
            //uiScores.DisplayWinText(team);
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
        Ball = (GameObject)Instantiate(ballPrefab, ballSpawnPoints[server].transform.position, Quaternion.identity);
        NetworkServer.Spawn(Ball);
        serviceManager.SetNewServiceZone(Utility.Opp(lastPointWinner));
        triggerNewBall = !triggerNewBall;
    }

    public int GetPlayerScore(Utility.Team team){
        return scores[team];
    }

    public void RelocateBall(Vector3 newPosition)
    {
        if (Ball == null)
        {
            UpdateBall();
        }
        Ball.transform.position = newPosition;
    }

    private void OnChangeTriggerPointWin(bool newVal)
    {
        Debug.Log("Trigger point win ok, waiting for players");
        
        foreach (Utility.Team team in Enum.GetValues(typeof(Utility.Team)))
        {
            // TODO better client side handling of this shit
            //PlayersReady[team] = false;
        }
        //IsWaitingForPlayers = true;
        IsWaitingForPlayers = false;
        MustStartNewPoint = true;
    }

    private void OnChangeLastPointWinner(Utility.Team newTeam)
    {
        scores[newTeam]++;
    }

    private void OnChangeCurrentServiceZoneIndex(int newIndex)
    {

        if (newIndex == -1)
        {
            Debug.Log("PlayerController: reset service zone mesh");
            // Remove the old service zone
            serviceZones[currentServiceZoneIndex].GetComponent<MeshRenderer>().enabled = false;
        } else
        {
            // Set the new service zone
            Debug.Log("PlayerController: set new service zone mesh");
            serviceZones[newIndex].GetComponent<MeshRenderer>().enabled = true;
        }
    }

    private void OnChangeTriggerNewBall(bool newVal)
    {

        Debug.Log("PlayerController trigger new Ball has changed value");
        UpdateBall();
    }

    public void UpdateBall()
    {
        Ball = GameObject.FindGameObjectWithTag("Ball");
    }
}
