using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathPopup : MonoBehaviour {

    public void Restart()
    {                                                               //this function will be linked to the click of the restart button
        if (GameManager2.Instance.OnGameReset != null)
        {
            GameManager2.Instance.OnGameReset.Invoke();             //invoke the event, to be picked up by the Level Generator
        }
        Destroy(this.gameObject);                                   //UI Manager will create new instance of the deathPopup prefab when needed again
    }

    public void MainMenu()
    {                                                               //this function will be linked to the click of the Main Menu button
        GameManager2.Instance.LoadMainMenu();
    }

    public static explicit operator GameObject(DeathPopup v)
    {
        throw new NotImplementedException();
    }
}
