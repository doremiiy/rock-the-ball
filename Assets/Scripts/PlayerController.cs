using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;




public class PlayerController : MonoBehaviour {

    public Utility.HandController handController;
    private VRNode vrNode;
    private Vector3 previousPosition = new Vector3();
    private Vector3 currentPosition = new Vector3();
    private Vector3 speed;
    public float forceMultiplier;

	void Start () {
        // TODO Change the data structure to Dictionnary
        switch (handController)
        {
            case Utility.HandController.right:
                vrNode = VRNode.RightHand;
                break;
            case Utility.HandController.left:
                vrNode = VRNode.LeftHand;
                break;
            default :
                Debug.Log("Unrecognized controller side");
                break;
        }
        previousPosition = transform.position;
    }

    private void FixedUpdate()
    {
        previousPosition = currentPosition;
        currentPosition = transform.position;
        speed = (currentPosition - previousPosition) / Time.fixedDeltaTime;
		transform.localPosition = InputTracking.GetLocalPosition(vrNode);
		transform.localRotation = InputTracking.GetLocalRotation(vrNode);
        //Debug.Log("Current speed:" + speed);

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ball"))
        {
            Debug.Log("Collision with the ball detected ");
            Rigidbody ballRigidbody = collision.collider.gameObject.GetComponent<Rigidbody>();
            Vector3 force = speed * forceMultiplier;
            ballRigidbody.AddForce(force);
        }
    }
}
