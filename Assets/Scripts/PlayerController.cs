using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VR;




public class PlayerController : NetworkBehaviour{

    [System.Serializable]
    public class HandManager : System.Object
    {
        public bool shouldUseVive;
        public GameObject hand;

		private PlayerController playerController;
        private Vector3 previousPosition = new Vector3();
        private Vector3 currentPosition = new Vector3();
        private Vector3 speed;
        private VRNode vrNode;
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
                playerController.BallHit(hand.GetComponent<RacketController>().handSide);
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

		public PlayerController PlayerController {
			get {
				return playerController;
			}
			set
			{
				playerController = value;
			}
		}
    }

    public GameManager gameManager;
    public float forceMultiplier;
    public HandManager rightHand, leftHand;
    public Camera playerCamera;
    public Utility.Team team;

    //[SyncVar(hook = "OnChangeBallPosition")]
    //private bool ballPositionTrigger;
    [SyncVar(hook = "OnChangeBallPosition") ]
    public Vector3 ballPosition;
    [SyncVar(hook = "OnChangeBallForce")]
    public Vector3 ballForce; 

    // TODO create a data structure in the gameManager to register every ball in the field
    //public GameObject ball;

    // TEST SECTION
    public Vector3 testForce;



    private void Start()
    {
        rightHand.VrNode = VRNode.RightHand;
        leftHand.VrNode = VRNode.LeftHand;
		rightHand.PlayerController = leftHand.PlayerController = this;

        // prevent camera swaping when a client joins
        if (!isLocalPlayer && playerCamera.enabled)
        {
            playerCamera.enabled = false;
        }

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

    }

    // TODO replace this initialization by a call to the gameManager to get the proper ball when applying a force
    public override void OnStartClient()
    {
        base.OnStartClient();
        //ball = GameObject.FindGameObjectWithTag("Ball");
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        //ball = GameObject.FindGameObjectWithTag("Ball");
    }


    private void FixedUpdate()
    {
        float timeLapse = Time.fixedDeltaTime;

        if (isLocalPlayer)
        {
            rightHand.Refresh(timeLapse, true);
            leftHand.Refresh(timeLapse, true);
        } else if (isServer)
        {
            rightHand.Refresh(timeLapse, false);
            leftHand.Refresh(timeLapse, false);
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

        // Test code to apply a force on the ball without a htc Vive
        if (isServer && isLocalPlayer && Input.GetKeyDown(KeyCode.Space))
        {
            GameObject ball = GameObject.FindGameObjectWithTag("Ball");
            Rigidbody rb_Ball = ball.GetComponent<Rigidbody>();
            ballPosition = ball.transform.position;
            //ballForce = !ballForceTrigger;  
        }
    }

    // Works only on the server
    // TODO replace the gameObject parameter with a enum value
    public void BallHit(Utility.Hand handSide)
    {
        if (!isServer)
        {
            return;
        }

        Debug.Log("Player Controller: Ball Hit called");

        Rigidbody ballRigidbody = gameManager.Ball.GetComponent<Rigidbody>();
        ballForce = Vector3.zero;

        if (handSide == rightHand.hand.GetComponent<RacketController>().handSide)
            ballForce = rightHand.Speed * forceMultiplier;
        else if (handSide == leftHand.hand.GetComponent<RacketController>().handSide)
            ballForce = leftHand.Speed * forceMultiplier;


        ballPosition = gameManager.Ball.transform.position;

        Debug.Log("Player Controller: Ball force is: " + ballForce);
        Debug.Log("Player Controller: Ball position is: " + ballPosition);
        // TODO replace the force application by an update of the ball's velocity
    }

    // TODO handle the situation for multiple balls in the Scene

    // The syncVar are changed only in 1 playerController => No condition to check. 
    private void OnChangeBallPosition(Vector3 newBallPosition)
    {
        Debug.Log("Player Controller: Ball position change detected, newPosition =" + newBallPosition);
        gameManager.RelocateBall(newBallPosition);
    }

    //TODO ballForce 
    private void OnChangeBallForce(Vector3 newBallForce)
    {
        Debug.Log("Player Controller: Ball force change detected =" + newBallForce);
        if (gameManager.Ball == null)
        {
            gameManager.UpdateBall();
        }
        gameManager.Ball.GetComponent<Rigidbody>().AddForce(newBallForce, ForceMode.Impulse);
    }

    // Only checked by the server
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

    // Only checked by the server
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
