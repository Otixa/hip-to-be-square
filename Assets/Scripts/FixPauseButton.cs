using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixPauseButton : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        UIManager.OnDialogOpen += HideMe;
        UIManager.OnDialogClose += ShowMe;
        //Debug.Log("I've subbed");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void HideMe()
    {
       // Debug.Log("Should be hiding pause menu");
        gameObject.SetActive(false);
    }
    public void ShowMe()
    {
       // Debug.Log("Should be showing pause menu");
        gameObject.SetActive(true);
    }

}
