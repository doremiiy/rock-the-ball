using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

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
    private SoundManager soundManager;

    // Local team
    private Utility.Team localTeam;
    
    // Score
    private Dictionary<Utility.Team, int> score;

    // Wait for players between points
    private Dictionary<Utility.Team, bool> playersReady;
    private bool isWaitingForPlayers;
    private bool mustStartNewPoint;

    // Training only
    private bool canAccessNextStep;
    private Utility.TrainingStep trainingStep;


    [SyncVar]
    private Utility.Team server;
    [SyncVar(hook = "OnChangeTriggerPointWin")]
    private bool triggerPointWin;
    [SyncVar(hook = "OnChangeTriggerGameWin")]
    private bool triggerGameWin;
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

    public bool CanAccessNextStep
    {
        get
        {
            return canAccessNextStep;
        }

        set
        {
            canAccessNextStep = value;
        }
    }

    public Utility.TrainingStep TrainingStep
    {
        get
        {
            return trainingStep;
        }

        set
        {
            trainingStep = value;
        }
    }

    public Utility.Team LocalTeam
    {
        get
        {
            return localTeam;
        }

        set
        {
            localTeam = value;
        }
    }

    private void Start()
    {
        Score = new Dictionary<Utility.Team, int>
        {
            { Utility.Team.BLUE, 0 },
            { Utility.Team.RED, 0 }
        };

        StartCoroutine(WaitForInitialization(0.1f));
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        serviceManager = GameObject.FindGameObjectWithTag("ServiceManager").GetComponent<ServiceManager>();
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
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
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        server = startSide; 
        
        // launch the training sequence
        if (GameState.training)
        {
            TrainingStep = Utility.TrainingStep.INITIAL;
            SwitchGoalActivation(false);
        }
    }

    private void Update()
    {
        //if (MustStartNewPoint && !IsWaitingForPlayers && Input.GetKeyDown(KeyCode.Return))
        if (Input.GetKeyDown(KeyCode.Return) && !GameState.training)
        {
            //MustStartNewPoint = false;
            StartNewPoint();
        }

        // TODO refactor all this logic in another class
        // TODO refactor the playerController code
        // Maybe a class to play without the network
        // Something with inheritance
        if (GameState.training)
        {
            if (CanAccessNextStep && (Input.GetButtonDown("Cancel") || Input.GetKeyDown(KeyCode.Return)))
            {
                TrainingStep++;
                CanAccessNextStep = false;
                StartNewTrainingPoint();
            }
        }

        if (Input.GetButtonDown("MainMenu"))
        {
            SceneManager.LoadScene("Menu");
        }
    }

    private void StartNewTrainingPoint()
    {
        uiManager.HideMainText();
        switch (TrainingStep)
        {
            case Utility.TrainingStep.INITIAL:
                CreateNewBall(LocalTeam);
                Ball.transform.Rotate(GameState.ballRotation);
                Ball.GetComponent<Ball>().SwitchBallUIActivation(true);
                soundManager.PlaySound("TrainingInitial");
                break;

            case Utility.TrainingStep.GOAL:
                Network.Destroy(Ball);
                CreateNewBall(LocalTeam);
                soundManager.PlaySound("TrainingGoal");
                SwitchGoalActivation(true);
                break;

            case Utility.TrainingStep.SERVICE:
                soundManager.PlaySound("TrainingService");
                StartNewPoint();
                break;

            case Utility.TrainingStep.FREE:
                CreateNewBall(server);
                soundManager.PlaySound("TrainingFree");
                SwitchGoalActivation(false);
                break;

            default:
                Debug.Log("GameManager: error, unsupported training step reached");
                break;
        }
    }

    private void CreateNewBall(Utility.Team team)
    {
        Ball = (GameObject)Instantiate(ballPrefab, ballSpawnPoints[team].transform.position, Quaternion.identity);
        NetworkServer.Spawn(Ball);
        triggerNewBall = !triggerNewBall;
    }

    public void ReplaceBall()
    {
        Network.Destroy(Ball);
        StartNewPoint();
    }

    private void StartNewPoint()
    {
        CreateNewBall(server);
        serviceManager.SetNewServiceZone(server);
    }

    public void IncreasePlayerScore(Utility.Team team)
    {
        if (!isServer)
        {
            return;
        }

        if (!GameState.training)
        {
            server = Utility.Opp(team);
        }
        else
        {
            server = LocalTeam;
        }
        IncrementScore(team);
        Network.Destroy(Ball);
        Debug.Log("Score: " + Score[team] + "Winning score : " + Utility.winningScore);
        if (Score[team] == Utility.winningScore)
        {
            triggerGameWin = !triggerGameWin;
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
        foreach (Utility.Team team in Enum.GetValues(typeof(Utility.Team)))
        {
            // TODO better client side handling of this shit
            //PlayersReady[team] = false;
        }
        //IsWaitingForPlayers = true;
        IsWaitingForPlayers = false;
        MustStartNewPoint = true;
    }

    private void OnChangeTriggerGameWin(bool newVal)
    {
        // Display winning text
        // Trigger or another button comes back to the main menu
        uiManager.ShowWinningText(GetWinningTeam());
    }

    // Should only called at the end of the match when there is no equality
    // TODO raise error in case of equality
    private Utility.Team GetWinningTeam()
    {
        int winningScore = 0;
        Utility.Team winningTeam = Utility.Team.BLUE;

        foreach (KeyValuePair<Utility.Team, int> entry in score)
        {
            if (entry.Value > winningScore)
            {
                winningScore = entry.Value;
                winningTeam = entry.Key;
            } 
        }

        return winningTeam;
    }

    private void OnChangeTriggerNewBall(bool newVal)
    {
        UpdateBall();
    }

    public void UpdateBall()
    {
        if (!GameState.training)
        {
            Ball = GameObject.FindGameObjectWithTag("Ball");
        }
    }

    private void IncrementScore(Utility.Team team)
    {
        score[team]++;
        uiManager.TeamScoreTrigger = team;
    }

    private void SwitchGoalActivation(bool isEnabled)
    {
        GameObject[] goals = GameObject.FindGameObjectsWithTag("Goal");

        // Disable goals and replace by a bouncing wall
        foreach (GameObject goal in goals)
        {
            goal.GetComponent<Goal>().isActive = isEnabled;
            goal.GetComponent<BouncingWall>().isActive = !isEnabled;
        }
    }

    public void AccessNextTrainingStep()
    {
        CanAccessNextStep = true;
        uiManager.ShowTrainingText();
    }

    IEnumerator WaitForInitialization(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Debug.Log("Local Team : " + LocalTeam);

        if (LocalTeam == Utility.Team.BLUE)
        {
            GameState.ballRotation = new Vector3(0f, 180f, 0f);
            GameState.mainUIRotation = new Vector3(0f, 180f, 0f);
        }
        else if (LocalTeam == Utility.Team.RED)
        {
            GameState.ballRotation = Vector3.zero;
            GameState.mainUIRotation = Vector3.zero;
        }
        else
        {
            Debug.Log("GameManager Error: unsupported team");
        }

        uiManager.SetUpMainUI();

        if (GameState.training)
        {
            CanAccessNextStep = false;
            StartNewTrainingPoint();
        }
    }
}
