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
        private Vector3 previousPosition = new Vector3();
        private Vector3 currentPosition = new Vector3();
        private Vector3 speed;
        private VRNode vrNode;

        public void Refresh(float timeLapse)
        {
            if (shouldUseVive)
            {
                hand.transform.localPosition = InputTracking.GetLocalPosition(VrNode);
                hand.transform.localRotation = InputTracking.GetLocalRotation(VrNode);
            }
            PreviousPosition = CurrentPosition;
            CurrentPosition = hand.transform.position;
            Speed = (CurrentPosition - PreviousPosition) / Time.fixedDeltaTime;
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
    }

    public float forceMultiplier;
    public HandManager rightHand, leftHand;
    public Camera playerCamera;

    private void Start()
    {
        rightHand.VrNode = VRNode.RightHand;
        leftHand.VrNode = VRNode.LeftHand;
    }

    private void FixedUpdate()
    {   
        if (!isLocalPlayer)
        {
            return;
        }

        float timeLapse = Time.fixedDeltaTime;
        
        rightHand.Refresh(timeLapse);
        leftHand.Refresh(timeLapse);
    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            playerCamera.enabled = false;
            return;
        }

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
    }


    public void BallHit(GameObject hand, Collider collider)
    {
        if (!collider.CompareTag("Ball"))
        {
            return;
        }
        Rigidbody ballRigidbody = collider.gameObject.GetComponent<Rigidbody>();
        Vector3 force = new Vector3();
        if (hand == rightHand.hand)
            force = rightHand.Speed * forceMultiplier;
        else if (hand == leftHand.hand)
            force = leftHand.Speed * forceMultiplier;
        ballRigidbody.AddForce(force);
    }
}
