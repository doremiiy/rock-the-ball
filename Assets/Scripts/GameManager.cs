using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class GameManager : NetworkBehaviour
{

    // Ball spawns
    public GameObject redPlayerBallSpawn;
    public GameObject bluePlayerBallSpawn;
    private Dictionary<Utility.Team, GameObject> ballSpawnPoints;
    // Ball prefab
    public GameObject ballPrefab;
    // Current ball
    public GameObject ball;
    
    // Managers
    public UIManager uiManager;
    private ServiceManager serviceManager;
    
    // Score
    private Dictionary<Utility.Team, int> score;

    // Wait for players between points
    private Dictionary<Utility.Team, bool> playersReady;
    private bool isWaitingForPlayers;
    private bool mustStartNewPoint;

    [SyncVar]
    private bool triggerGameWin;
    [SyncVar]
    private Utility.Team server;
    [SyncVar(hook = "OnChangeTriggerPointWin")]
    private bool triggerPointWin;
    [SyncVar(hook = "OnChangeTriggerNewBall")]
    private bool triggerNewBall;

    // Debug only, allow to choose the service side
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

    public Dictionary<Utility.Team, int> Score
    {
        get
        {
            return score;
        }

        set
        {
            score = value;
        }
    }

    private void Start()
    {
        Score = new Dictionary<Utility.Team, int>
        {
            { Utility.Team.BLUE, 0 },
            { Utility.Team.RED, 0 }
        };
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        serviceManager = GameObject.FindGameObjectWithTag("ServiceManager").GetComponent<ServiceManager>();
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        Ball = GameObject.FindGameObjectWithTag("Ball");
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        PlayersReady = new Dictionary<Utility.Team, bool>
        {
            { Utility.Team.BLUE, true},
            { Utility.Team.RED, true}
        };

        ballSpawnPoints = new Dictionary<Utility.Team, GameObject>
        {
            { Utility.Team.BLUE, bluePlayerBallSpawn },
            { Utility.Team.RED, redPlayerBallSpawn }
        };
        serviceManager = GameObject.FindGameObjectWithTag("ServiceManager").GetComponent<ServiceManager>();
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        server = startSide;
    }

    private void Update()
    {
        if (!isServer)
        {
            return;
        }

        //if (MustStartNewPoint && !IsWaitingForPlayers && Input.GetKeyDown(KeyCode.Return))
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //MustStartNewPoint = false;
            StartNewPoint();
        }
    }

    private void StartNewPoint()
    {
        Ball = (GameObject)Instantiate(ballPrefab, ballSpawnPoints[server].transform.position, Quaternion.identity);
        NetworkServer.Spawn(Ball);
        serviceManager.SetNewServiceZone(server);
        triggerNewBall = !triggerNewBall;
    }

    public void IncreasePlayerScore(Utility.Team team)
    {
        server = Utility.Opp(team);
        Score[team]++;
        IncrementScore(team);
        Network.Destroy(Ball);
        //uiScores.UpdateScores();
        if (Score[team] == Utility.winningScore)
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

    private void OnChangeTriggerNewBall(bool newVal)
    {
        UpdateBall();
    }

    public void UpdateBall()
    {
        Ball = GameObject.FindGameObjectWithTag("Ball");
    }

    private void IncrementScore(Utility.Team team)
    {
        score[team]++;
        uiManager.TeamScoreTrigger = team;
    }
}
