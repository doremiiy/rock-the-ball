using UnityEngine;
using UnityEngine.Networking;



// Responsabilité du service manager ? 
// Connait toutes les services zones.
// Connait l'état du service actuel
// Gère l'actualisation de ces zones
// Communique avec le gameManager et les serviceZones, pas avec la ball

// Le gameManager connait le service Manager
// Lors de son start, le gameManager créée un service Manager 
// knows the player who is serving
// gives point ? 


public class ServiceManager : NetworkBehaviour {

    private GameObject[] serviceZones;
    private GameObject currentServiceZone;
    [SyncVar (hook ="OnChangeCurrentServiceZoneIndex")]
    private int currentServiceZoneIndex;

    private bool isServed;
    private Utility.Team servingPlayer;

    private GameManager gameManager;

    public GameManager GameManager
    {
        get
        {
            return gameManager;
        }

        set
        {
            gameManager = value;
        }
    }

    void Start () {
        
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        serviceZones = GameObject.FindGameObjectsWithTag("ServiceZone");
        foreach (GameObject serviceZone in serviceZones)
        {
            serviceZone.GetComponent<ServiceZone>().ServiceManager = this;
        }
        serviceZones[currentServiceZoneIndex].GetComponent<MeshRenderer>().enabled = true;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        serviceZones = GameObject.FindGameObjectsWithTag("ServiceZone");
        foreach (GameObject serviceZone in serviceZones)
        {
            serviceZone.GetComponent<ServiceZone>().ServiceManager = this;
        }
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void HandleService(bool isIn)
    {
        if (!isServed)
        {
            return;
        }

        if (isIn)
        {
            Debug.Log("Service In !");
            ResetServiceZone();
        } else
        {
            Debug.Log("Service Out !");
            GameManager.IncreasePlayerScore(Utility.Opp(servingPlayer));
        }
        isServed = false;
    }


    public void SetNewServiceZone(Utility.Team sp)
    {
        servingPlayer = sp;
        isServed = true;
        ResetServiceZone();
        currentServiceZoneIndex = UnityEngine.Random.Range(0, serviceZones.Length);
        currentServiceZone = serviceZones[currentServiceZoneIndex];
        currentServiceZone.GetComponent<ServiceZone>().IsValid = true;
    }


    public void ResetServiceZone()
    {
        currentServiceZone.GetComponent<ServiceZone>().IsValid = false;
        currentServiceZoneIndex = -1;
    }

    private void OnChangeCurrentServiceZoneIndex(int newIndex)
    {
        if (newIndex == -1)
        {
            Debug.Log("ServiceManager: reset service zone mesh");
            serviceZones[currentServiceZoneIndex].GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            // To set a new ServiceZone, the service zone are supposed to be reset first
            if (currentServiceZoneIndex != -1)
            {
                serviceZones[currentServiceZoneIndex].GetComponent<MeshRenderer>().enabled = false;
            }
            Debug.Log("ServiceManager: set new service zone mesh");
            serviceZones[newIndex].GetComponent<MeshRenderer>().enabled = true;
        }
    }
}