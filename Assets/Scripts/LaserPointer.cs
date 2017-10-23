using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour {

	private LineRenderer laserLineRenderer;
	public float laserWidth = 0.1f;
	public float laserMaxLength = 5f;

	void Start ()
	{
		laserLineRenderer = GetComponent<LineRenderer> ();
		laserLineRenderer.SetWidth (laserWidth, laserWidth);
	}		

	void FixedUpdate()
	{
		Ray ray = new Ray (transform.position, transform.forward);
		//RaycastHit raycastHit;
		laserLineRenderer.SetPosition( 0, transform.position );
		laserLineRenderer.SetPosition( 1, transform.forward );
		//RaycastHit hit;
		//R
		//Debug.DrawRay (transform.position, transform.forward);
		//if (Physics.Raycast(ray, out hit))
		//	Debug.Log("There is something in front of the object!");
	}
}
