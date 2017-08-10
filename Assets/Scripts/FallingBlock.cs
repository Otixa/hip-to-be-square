using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class FallingBlock : MonoBehaviour {
	private Rigidbody2D myRigidbody;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Awake () {
		myRigidbody = GetComponent<Rigidbody2D> ();
		myRigidbody.gravityScale = 0;
		myRigidbody.bodyType = RigidbodyType2D.Kinematic;

	}


	void OnCollisionEnter2D(Collision2D other){
		if (other.gameObject.tag =="Player" && other.gameObject.name != "Whitey"){
			Debug.Log ("Game object name is " + other.gameObject.name);
			//Time.timeScale = 0.2f;
			myRigidbody.gravityScale = 1;
			myRigidbody.bodyType = RigidbodyType2D.Dynamic;
		}
	}

}
