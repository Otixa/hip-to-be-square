using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager2 : Singleton<GameManager2>
{
    /* To do: 
     * 
     * Ensure player resets his position correctly upon death / reset Resettable OBject should do this - only if he is enabled / disabled.
     * Ensure that buffPickups on Reset check if they are invoking, and if so - call cancelBuff()
     * Upon player death, set a death animation (instead of setting Active to be false)
     * On death, we no longer call Restart Game. We launch death menu from UI manager which then call the GameManager reset function
     * On restart, GenericPlayer needs to reset his focus, position, animation
     * UI manager needs to retrieve the information about score / player / buffs in order to display it on the UI.
     * We need to send event to Level Generator when score reaches the Cap, so that it can spawn the home straight (end of level) section.
     * UI needs to know when a buff in invoked in order to display the buff duration and buff type
     */

    /* ------------------ REFERENCE TO OTHER MANAGERS / SCRIPTS ----------------- */
    // -- OH THERE ARE NO LONGER ANY #REFACTORING AND MODULARISING LIKE A BOSS -- //

    /* --------------------- FIELDS USED BY THE GAME MANAGERS -------------------- */
    
    public float scoreCounter = 0;                  //this is used to keep track of the current score of this game (from start until death)
    public float pointsPerSecond = 10;              //this specifies how many points we should add to the score every second that passes
    [Range(100, 1000)] public float scoreCap = 1000;                      //the limit of points needed to be earned to unlock the level exit
    public bool scoringEnabled = true;          //this boolean controls whether or not points should be added to the score counter or not (when you die, points should stop increasing)
    public bool spawnedEnd;                     //boolean that ensures the endoflevel code (EVENT) is only executed (BROADCASTED) once
    public int attemptCounter;                    //does what it says on the tin..!
    public string nextSceneName;                //Scene nextLevel; 
    public string mainMenuScene = "TitleScreen";//a way to speicify the name of the main menu scene
    public static Action OnLevelFinish;
    public static Action<MonoBehaviour> OnPlayerDeath;
    public static Action OnGameReset;

    private GameManager2() { }

    private void Awake()
    {
        scoreCounter = 0;
        OnPlayerDeath += OnDeath;
        OnGameReset += ResetGame;
    }

    //handle the scoring in here
    void Update() {
        //INCREASE SCORE
        if (scoringEnabled)
        {                                                                   //if we are alive and not finished the level !!Maybe don't need this if we just clamp / cap the score
            scoreCounter = Mathf.Clamp(scoreCounter + (pointsPerSecond * Time.deltaTime), 0, scoreCap);             //calculate score per frame and add it to our counter
        }
        //CHECK IF WE HAVE MET WIN CONDITION
        if (scoreCounter >= scoreCap && spawnedEnd == false)
        {               //check the score to see if it is ready to end the level  
            spawnedEnd = true;                                              //to ensure this if statement only executes once
            scoringEnabled = false;
            if (OnLevelFinish != null) { 
                OnLevelFinish.Invoke();             //invoke the event, to be picked up by the Level Generator
            }
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void AddPoints(int amount)
    {                                       //a way for other scripts to affect the score
        scoreCounter = Mathf.Clamp(scoreCounter + amount, 0, scoreCap);     //add the amount passed in to the current score
    }
    //should we add anti-pickups that reduce your score, we should add a RemovePoints( ) also.

    public void OnDeath(MonoBehaviour cause)
    {
        scoringEnabled = false;
        Time.timeScale = 0f;
        //we add to attempts on game reset, to avoid exploits
    } 

    //this is the set of actions to perform when we need to restart the game from the start of the level
    public void ResetGame()
    {
        scoreCounter = 0;
        if (BuffPickup.GetActive() != null)
        {
            BuffPickup.GetActive().Cancel();
        }
        scoreCounter = 0;                              //set score back to 0 ready for a new game,             
        scoringEnabled = true;                         //re-enable scoring once the new game begin             !!if distance travelled, then won't need this
        spawnedEnd = false;                            //reset this, allowing us to meet the winning criteria again
        Time.timeScale = 1f;                           //reset the time to regular speed, incase player died during slow down      !!this should happen upon buff resetting? 
    }

    public void TriggerReset()
    {
        if(OnGameReset != null)
        {
            OnGameReset.Invoke();
        }
    }

}
