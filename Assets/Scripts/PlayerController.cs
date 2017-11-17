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


        public void Refresh(float timeLapse)
        {
            if (shouldUseVive)
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
                playerController.BallHit(hand, hit.collider);
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

    //[SyncVar(hook = "OnChangeBallPosition")]
    //private bool ballPositionTrigger;
    [SyncVar(hook = "OnChangeBallPosition") ]
    public Vector3 ballPosition;
    [SyncVar(hook = "OnChangeBallForce")]
    private bool ballForceTrigger;
    public Vector3 ballForce; 

    // TODO create a data structure in the gameManager to register every ball in the field
    public GameObject ball;

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
        ball = GameObject.FindGameObjectWithTag("Ball");
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        ball = GameObject.FindGameObjectWithTag("Ball");
    }


    private void FixedUpdate()
    {   

        float timeLapse = Time.fixedDeltaTime;

        rightHand.Refresh(timeLapse);
        leftHand.Refresh(timeLapse);            
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
            Debug.Log("OK");
            GameObject ball = GameObject.FindGameObjectWithTag("Ball");
            Rigidbody rb_Ball = ball.GetComponent<Rigidbody>();
            ballPosition = ball.transform.position;
            ballForceTrigger = !ballForceTrigger;  
        }
    }

    // Works only on the server
    public void BallHit(GameObject hand, Collider collider)
    {
        if (!isServer || !collider.CompareTag("Ball"))
        {
            return;
        }

        Debug.Log("PLayer controller ok");
        Rigidbody ballRigidbody = collider.gameObject.GetComponent<Rigidbody>();

        if (hand == rightHand.hand)
            ballForce = rightHand.Speed * forceMultiplier;
        else if (hand == leftHand.hand)
            ballForce = leftHand.Speed * forceMultiplier;

        ballForceTrigger = !ballForceTrigger;
        ballPosition = ball.transform.position;
        // TODO replace the force application by an update of the ball's velocity
    }

    // TODO handle the situation for multiple balls in the Scene

    // The syncVar are changed only in 1 playerController => No condition to check. 
    private void OnChangeBallPosition(Vector3 newBallPosition)
    {
        gameManager.RelocateBall(newBallPosition);
    }

    private void OnChangeBallForce(bool newVal)
    {
        ball.GetComponent<Rigidbody>().AddForce(ballForce, ForceMode.Impulse);  
    }
}
