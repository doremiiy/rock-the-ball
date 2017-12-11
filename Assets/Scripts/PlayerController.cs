using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VR;




public class PlayerController : NetworkBehaviour{

    [System.Serializable]
    public class HandManager : System.Object
    {
        // For debug without a Vive
        public bool shouldUseVive;

        private PlayerController playerController;
        public GameObject hand;
        // Vive controller to associate
        private VRNode vrNode;

        // Tracking of the speed
        private Vector3 previousPosition = new Vector3();
        private Vector3 currentPosition = new Vector3();
        private Vector3 speed;
        // Layer mask for the collision fix
        public LayerMask castMask;
        
        public void Refresh(float timeLapse, bool shouldTrackPosition)
        {
            if (shouldUseVive && shouldTrackPosition)
            {
                hand.transform.localPosition = InputTracking.GetLocalPosition(VrNode);
                hand.transform.localRotation = InputTracking.GetLocalRotation(VrNode);
            }

            PreviousPosition = CurrentPosition;
            CurrentPosition = hand.transform.position;
            Speed = (CurrentPosition - PreviousPosition) / timeLapse;

            RaycastHit hit;
            if (Physics.Linecast(previousPosition, currentPosition, out hit, castMask))
            {
                hand.transform.position = hit.point;
                currentPosition = hand.transform.position;
                playerController.BallHit();
            }   
        }

        public Vector3 PreviousPosition
        {
            get
            {
                return previousPosition;
            }

            set
            {
                previousPosition = value;
            }
        }

        public Vector3 CurrentPosition
        {
            get
            {
                return currentPosition;
            }

            set
            {
                currentPosition = value;
            }
        }

        public Vector3 Speed
        {
            get
            {
                return speed;
            }

            set
            {
                speed = value;
            }
        }

        public VRNode VrNode
        {
            get
            {
                return vrNode;
            }

            set
            {
                vrNode = value;
            }
        }

		public PlayerController PlayerController
        {
			get
            {
				return playerController;
			}
			set
			{
				playerController = value;
			}
		}
    }

    public GameManager gameManager;
    
    // Player controller
    public HandManager firstHand;
    // Player camera
    public Camera playerCamera;
    // Player team
    public Utility.Team team;
    // Player spawn
    public GameObject playerSpawn;

    public float forceMultiplier;

    // TODO move this logic in a new Ball Manager class
    // Ball force trigger
    [SyncVar(hook = "OnChangeBallPosition") ]
    public Vector3 ballPosition;
    [SyncVar(hook = "OnChangeBallForce")]
    public Vector3 ballForce; 

    private void Start()
    {
        firstHand.VrNode = VRNode.RightHand;
		firstHand.PlayerController =  this;

        if (!isLocalPlayer && playerCamera.enabled)
        {
            playerCamera.enabled = false;
            GetComponent<AudioListener>().enabled = false;
        }

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
       
        GameObject redPlayerSpawn = GameObject.FindGameObjectWithTag("RedPlayerSpawn");
        GameObject bluePlayerSpawn = GameObject.FindGameObjectWithTag("BluePlayerSpawn");
            
        if (redPlayerSpawn.transform.position == transform.position)
        {
            GameState.trainingTeam = Utility.Team.RED;
            team = Utility.Team.RED;
            playerSpawn = redPlayerSpawn;
            Debug.Log("playerController: redPlayerSpawn found");
        } else if (bluePlayerSpawn.transform.position == transform.position)
        {
            GameState.trainingTeam = Utility.Team.BLUE;
            team = Utility.Team.BLUE;
            playerSpawn = bluePlayerSpawn;
            Debug.Log("playerController: bluePlayerSpawn found");
        }
        else
        {
            Debug.Log("PlayerController ERROR: The player spawn wasn't found");
            Debug.Log("redPlayerSpawn = " + redPlayerSpawn.transform.position);
            Debug.Log("bluePlayerSpawn = " + bluePlayerSpawn.transform.position);
            Debug.Log("player position = " + transform.position);
        }
    }


    private void FixedUpdate()
    {
        float timeLapse = Time.fixedDeltaTime;

        if (isLocalPlayer)
        {
            firstHand.Refresh(timeLapse, true);
        } else if (isServer)
        {
            firstHand.Refresh(timeLapse, false);
        }
    }

    private void Update()
    {

        if (!isLocalPlayer)
        {
            return;
        }

        // Test code to move the players without a htc vive
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
    }

    public void BallHit()
    {
        if (!isServer)
        {
            return;
        }

        // TODO replace the force application by an update of the ball's velocity
        Rigidbody ballRigidbody = gameManager.Ball.GetComponent<Rigidbody>();
        ballForce = Vector3.zero;
        ballForce = firstHand.Speed * forceMultiplier;
        ballPosition = gameManager.Ball.transform.position;

        // If in training mode, allows to go the next step
        if (GameState.training && gameManager.TrainingStep == Utility.TrainingStep.INITIAL)
        {
            gameManager.CanAccessNextStep = true;
        }
    }

    private void OnChangeBallPosition(Vector3 newBallPosition)
    {
        gameManager.RelocateBall(newBallPosition);
    }

    private void OnChangeBallForce(Vector3 newBallForce)
    {
        if (gameManager.Ball == null)
        {
            gameManager.UpdateBall();
        }
        gameManager.Ball.GetComponent<Rigidbody>().AddForce(newBallForce, ForceMode.Impulse);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isServer)
        {
            return;
        }

        if (gameManager.IsWaitingForPlayers && other.CompareTag("WaitingZone"))
        {
            //TODO vive controllers trigger
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("Player ready");
                gameManager.PlayersReady[team] = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isServer)
        {
            return;
        }

        if (other.CompareTag("WaitingZone") && gameManager.MustStartNewPoint)
        {
            Debug.Log("player not ready");
            gameManager.PlayersReady[team] = false;
        }
    }
}
