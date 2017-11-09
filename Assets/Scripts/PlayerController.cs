using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.VR;




public class PlayerController : NetworkBehaviour{

    [System.Serializable]
    public class HandManager : System.Object
    {
        public GameObject hand;
        private Vector3 previousPosition = new Vector3();
        private Vector3 currentPosition = new Vector3();
        private Vector3 speed;

        public void Refresh(float timeLapse)
        {
            PreviousPosition = CurrentPosition;
            CurrentPosition = hand.transform.position;
            Speed = (CurrentPosition - PreviousPosition) / timeLapse;
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
    }

    public float forceMultiplier;
    public HandManager rightHand, leftHand;
    public Camera playerCamera;

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

    // replace the temmporary update solution set up to prevent camera swaping on client connection
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!isLocalPlayer && playerCamera.enabled)
        {
            playerCamera.enabled = false;
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


    public void BallHit(GameObject hand, Collider collider)
    {
        Debug.Log("Trigger Detected");
        if (!collider.CompareTag("Ball"))
        {
            return;
        }
        Debug.Log("Collision with the ball detected ");
        Rigidbody ballRigidbody = collider.gameObject.GetComponent<Rigidbody>();
        Vector3 force = new Vector3();
        if (hand == rightHand.hand)
            force = rightHand.Speed * forceMultiplier;
        else if (hand == leftHand.hand)
            force = leftHand.Speed * forceMultiplier;
        Debug.Log(force);
        ballRigidbody.AddForce(force);
    }
}
