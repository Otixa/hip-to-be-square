using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneScroller : MonoBehaviour {
	public float scrollOffset;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		
	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "scrollable") {
			other.transform.position = 
				new Vector3 (other.transform.position.x + scrollOffset, 
				other.transform.position.y, other.transform.position.z);
		}
	}
}
	