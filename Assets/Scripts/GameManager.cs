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
    private GameObject ball;

    private Dictionary<Utility.Team, int> scores;
    private Dictionary<Utility.Team, GameObject> ballSpawnPoints;
    private Dictionary<Utility.Team, GameObject[]> serviceZones;
    private ServiceZone currentServiceZone;
 

    private void Start()
    {
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


    private void FixedUpdate()
    {
        ball.GetComponent<Rigidbody>().AddForce(Vector3.forward * Time.fixedDeltaTime * 3f, ForceMode.Impulse);
    }

    public void IncreasePlayerScore(Utility.Team team)
    {
        int oldScore = scores[team];
        scores[team] = ++oldScore;
        uiScores.UpdateScores();
        if (oldScore == 15)
        {
            uiScores.DisplayWinText(team);
            // TODO add the end of the game
        }
        else
        { 
            StartNewPoint(team);
        }
    }

    private void StartNewPoint(Utility.Team wonLastPoint)
    {
        // A new ball is instantiated in front of the player who lost the last point
        var newBall = (GameObject)Instantiate(ballPrefab, ballSpawnPoints[Utility.Opp(wonLastPoint)].transform.position, Quaternion.identity);
        NetworkServer.Spawn(newBall);
        int rand = Random.Range(0, serviceZones[wonLastPoint].Length);
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

    private void SpawnBall()
    {
        ball = (GameObject)Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(ball);
    }
}
