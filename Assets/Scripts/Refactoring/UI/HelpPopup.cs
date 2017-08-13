using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpPopup : MonoBehaviour {
    private GenericPlayer player;

	// Use this for initialization
	void Start () {
        //pause time
        player = FindObjectOfType<GenericPlayer>();
        player.enabled = false;
        Time.timeScale = 0f;
        
	}
	
	// Update is called once per frame
	void Update () {
        //check if space has been pressed, if so, reset time and destroy this object
        if (Input.GetButtonUp("Jump"))
        {
            Time.timeScale = 1f;
            Destroy(gameObject);
            player.enabled = true;
        }
	}
}
