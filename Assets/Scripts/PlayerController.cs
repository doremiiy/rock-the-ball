using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;




public class PlayerController : MonoBehaviour {

    public Utility.HandController handController;
    private VRNode vrNode;

	void Start () {
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
    }
	
	void Update () {
        transform.position = InputTracking.GetLocalPosition(vrNode);
        transform.rotation = InputTracking.GetLocalRotation(vrNode);
    }
}
