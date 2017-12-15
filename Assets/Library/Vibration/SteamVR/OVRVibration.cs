using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.VR;

public class OVRVibration : MonoBehaviour {

	/// <summary>
	/// Vibrates the controller.
	/// </summary>
	/// <param name="controller">Controller node</param>
	/// <param name="time">duration in seconds</param>
	/// <param name="force">Force between 0 and 500</param>
	public void VibrateController(VRNode controller,int time,ushort force){
		var system = OpenVR.System;
		if (system != null)
		{
			
			if (controller == VRNode.LeftHand)
			{
				StartCoroutine(Vibrate(system.GetTrackedDeviceIndexForControllerRole (ETrackedControllerRole.LeftHand), time, force));

			}
			else if (controller == VRNode.RightHand){
				
				StartCoroutine(Vibrate(system.GetTrackedDeviceIndexForControllerRole (ETrackedControllerRole.RightHand), time, force));
			}
			else{
				
				print("BAD PARAMETERS");
			}

	
		}
	}

	IEnumerator Vibrate(uint id,int time,ushort force){

		float timeStart = Time.realtimeSinceStartup; 
		while(Time.realtimeSinceStartup - timeStart < time)
			OpenVR.System.TriggerHapticPulse(id, 0, (char)force);
		yield return null;
	}


	void Update(){
		//Example
		/*
		if(Input.GetKeyDown(KeyCode.I)){
			VibrateController (VRNode.LeftHand, 5,500);
		}*/
	}


}
