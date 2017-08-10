using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This component will be attached to things that you want to be destroyed once it's behind the
 *destruction point (which is off screen to the left of the player). This is to save memory. */
public class ObjectDestroyer : MonoBehaviour {
	//QUESTION: why don't we make this a transform, since it's just a transform?
	private Transform destructionPoint;											//Reference to the point objects need to pass in order to be deemed safe to destroy
	public bool keepActive = false;

	void Start () {
		destructionPoint = GameObject.Find ("DestructionPoint").transform;				//find the destruction point object that's attached to camera
	}

	void Update () {
		if (transform.position.x < destructionPoint.position.x) {		//if whatever has this script attached to it, is behind the destruction point
			gameObject.SetActive(false);										//deactivate so that we can re use them with pooling
		}
	}

//	void OnDrawGizmos(){
//		Gizmos.color = Color.red;
//		Gizmos.DrawSphere (transform.position, 2);
//	}



}