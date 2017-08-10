using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraX : MonoBehaviour {
	private CameraController theCameraController;			//we need a reference so that we can follow it's movement
	private float deltaX;										//this is x offset between camera and scenery 
	// Use this for initialization
	void Start () {
		theCameraController = FindObjectOfType<CameraController> ();
		deltaX = transform.position.x - theCameraController.transform.position.x;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (theCameraController.transform.position.x + deltaX, 
			transform.position.y, transform.position.z);
	}
}
