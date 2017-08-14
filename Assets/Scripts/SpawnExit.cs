using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnExit : MonoBehaviour {
	private GameObject theExit;			//reference to the level exit;
	public float spawnOffsetX;			//this defines how far along from the object with this component attached to spawn the exit (on the x axis)
    public float spawnOffsetY;          //this defines how far along from the object with this component attached to spawn the exit (on the x axis)

    // Use this for initialization
    void Start () {
		theExit = GameObject.Find ("LevelExit");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Player") {
			theExit.transform.position = new Vector3 (transform.position.x + spawnOffsetX, transform.position.y + spawnOffsetY, transform.position.z);
		}
	}
}
