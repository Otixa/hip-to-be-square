using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartOfTheScenery : MonoBehaviour {
	public Vector3 startPos;
	// Use this for initialization
	void Start () {
		startPos = transform.position;
	}

    void OnEnable()
    {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDisable()
    {
        startPos = transform.position;
    }
}
