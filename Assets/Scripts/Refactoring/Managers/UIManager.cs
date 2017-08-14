using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


[RequireComponent(typeof(Canvas))]
public class UIManager : Singleton<UIManager>
{

    public Text playerName;                             //get reference to the UI text object that displays the player name
    public Text currentScore;                           //get reference to the UI text object that displays the current score
    public Text focusAmount;                            //get reference to the UI text object that displays the slow speed points that remain
    public Text attemptCount;                           //display how many deaths 
    public Text buffDurationText;                       //get reference to the text UI object that will show the current duration of buff
    public Image buffImage;                             //reference to the image component that will display the larger buff type image
    public Sprite[] buffImages;                         //to store the images for the buff UI#
    public GameObject speedBar;                         //reference to the UI image that represents how much time slow down points we have remaining (the blue bar)
    public GameObject pointsBar;                        //reference to the image that represents the numbers of points gained
    public GameObject deathMenu;                        //deathMenu is a game object with image and text and clickable button (restart, menu button)
    //public PausePopup pauseMenu;                      //pauseMenu is game object with image, text and clickable buttons (resume, restart, menu button)
    //public TextOnlyPopup textPopup;                   //used to display an image background with text only (template includes instruction on how to dismiss the message).
    //public TextAndImagePopup textPopup;               //used to display an image background, an image and some text (template includes instruction on how to dismiss the message).
    //public TextAndContinuePopup continuePopup;        //used to dislpay text, and a button that when clicked will load a new scene
    private GenericPlayer player;                       //reference needed so we can access character name and speed bar value
    private LevelGenerator levelGenerator;              //reference needed so we can stop generating map when we accumulate enough points
    private float buffDurationCounter;                  //this used to keep track of remaining time and then display in the GUI

    public Canvas UICanvas;
    public static Action<GameObject> OnDialogDismiss;       //!!NEED TO MAKE EVENTS STATIC IF ON A SINGLETON
    public static Action OnDialogOpen;
    public static Action OnDialogClose;
    //from tutorial manager
    //public Text tut_text;
    //public bool popupActive;
    //public GameObject tutorialMessage;

    private UIManager() { }                             //Constructor needed for Singleton structures

    private void Awake()
    {
       
        GameManager2.OnPlayerDeath += OnDeath;     //subscribe to the event OnPlayerDeath, and run the OnDeath() function when invoked
        GameManager2.OnBuffExpire += HideBuff;     //subscribe to the event OnPlayerDeath, and run the OnDeath() function when invoked
        GameManager2.OnBuffPickup += SetBuff;     //subscribe to the event OnPlayerDeath, and run the OnDeath() function when invoked

        OnDialogDismiss += TestingEvent;
    }

    public void TestingEvent(GameObject go)
    {
        //Debug.Log("I am a test event, what are you?");
    }

    void Start()
    {
        UICanvas = GetComponent<Canvas>();
        player = FindObjectOfType<GenericPlayer>();                         //get hold of the player                !!is there another way around having to find the player, probably not
        levelGenerator = FindObjectOfType<LevelGenerator>();                //get hold of the MapGenerator
        playerName.text = player.playerStats.name;                          //grab the name field from the player                                           
        focusAmount.text = "Focus: " + player.playerStats.focus.ToString("F2") + " / " + player.playerStats.focus;  //display our speed points text (under the blue bar)
        buffImage.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);                //sets the buff image to be greyed out when inactive
        buffDurationText.color = new Color(1f, 1f, 1f, 0f);                 //sets the buff text to be invisible inactive
    }


    // Update is called once per frame
    void Update()
    {
        currentScore.text = "Score: " + GameManager2.Instance.scoreCounter.ToString("F0");              //update the text on the UI to show the score
        playerName.text = player.playerStats.name;                                                      //update the text on the UI to show the name (this may change mid game)
        //need to ensure scoreCounter isn't 0, else you're dividing by zero - inline if statement
        pointsBar.transform.localScale = new Vector3(GameManager2.Instance.scoreCap == 0 ? 0f : (GameManager2.Instance.scoreCounter / GameManager2.Instance.scoreCap), pointsBar.transform.localScale.y, pointsBar.transform.localScale.z);
        focusAmount.text = "Focus: " + Mathf.Round(player.playerStats.focus)/*.ToString("F2")*/+ " / " + player.maxFocus;       //update the text that shows how much slow time points that remain                                                                                                                           //adjusts the scale of the coloured portion of the HP bar. scale of 1 = full, so divide amount left by max amount to get a normalised number to pass into this
        speedBar.transform.localScale = new Vector3(player.playerStats.focus == 0 ? 0f : (player.playerStats.focus / player.maxFocus), speedBar.transform.localScale.y, speedBar.transform.localScale.z);
        //update and diplay the buff duration
        if (buffDurationCounter > 0)
        {
            buffDurationCounter -= Time.unscaledDeltaTime;
        }
        buffDurationText.text = "" + Mathf.Round(buffDurationCounter);
    }

    public void SetBuff(BuffPickup buff)
    {       
        if (BuffPickup.GetActive() is FocusBuff)             //BLUE BUFF
        {
            buffImage.GetComponent<Image>().overrideSprite = buffImages[0];
            buffImage.color = new Color(1f, 1f, 1f, 1f);
        }
        else if (BuffPickup.GetActive() is PointBuff)      //GREEN BUFF
        {
            buffImage.GetComponent<Image>().overrideSprite = buffImages[1];
            buffImage.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            Debug.Log(BuffPickup.GetActive());
            Debug.Log("Unrecognised Buff Type, unable to set correct image");
        }
        buffDurationText.color = new Color(1f, 1f, 1f, 1f);
        buffDurationCounter = buff.GetDuration();
    }

    public void HideBuff(BuffPickup buff)
    {
        if(buff == BuffPickup.GetActive() || BuffPickup.GetActive() == null)
        {
            buffImage.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            buffDurationText.color = new Color(1f, 1f, 1f, 0.1f);
        }

    }

    //This function is called on the event of OnPlayerDeath
    private void OnDeath(MonoBehaviour cause)
    {
        string deathMessage = "Unable to get deathmessage from event";
        //deathMenu.gameObject.SetActive(true);
        if (cause is FatalObject)
        {
            deathMessage = ((FatalObject)cause).GetDeathMessage();
        }
        //Debug.Log(deathMessage);
        UIPopup ui = new newDeathPopup(deathMessage);
        ui.Show();
        //LoadPopup((GameObject)deathMenu, deathMessage /*, ACTION CALLBACK HERE*/);
    }

    //public void LoadPopup(GameObject popupType, string message/*, Action Callback*/)
    //{
    //    //var popup = Instantiate(popupType, Vector3.zero, Quaternion.identity, transform.parent) as GameObject;
    //    var popup = Instantiate(popupType, this.transform.position, Quaternion.identity, this.transform) as GameObject;
    //    popup.GetComponentInChildren<Text>().text = message;                //there are more than one text component, we need a way of being more specific

    //    //float timeoutTimer = 10f;
    //    //Time.timeScale = 0f;
        
    //    //while (!Input.GetButtonDown("Jump") || timeoutTimer > 0)
    //    //{
    //    //    Debug.Log("I'm in the message popup timer loop");
    //    //    timeoutTimer -= Time.unscaledDeltaTime;
    //    //}
    //    //Time.timeScale = 1f;
    //    //Debug.Log("I'm out of it!");
    //    //Destroy(popup);
    //    //Callback.Invoke();                                   //Invoke the code we want to run once the popup has been loaded
    //    // we need a way to stop time, and to reenable time and player script upon input / button click. disable the popup,
    //}

    public GameObject CreatePopupByResourceName(string resourceName)
    {
        GameObject prefab = Resources.Load<GameObject>("UI/" + resourceName) as GameObject;
        GameObject uip = Instantiate(prefab, UICanvas.transform.position, UICanvas.transform.rotation, UICanvas.transform) as GameObject;
        uip.SetActive(false);
        return uip;
    }

    public void DestroyPopup(GameObject popup)
    {
        //Debug.Log("BUTTON PRESSED");
        //Debug.Log(OnDialogDismiss);
        if (OnDialogDismiss != null)
        {
           // Debug.Log("INVOKING FROM UI MANAGER");
            OnDialogDismiss.Invoke(popup);
            
        }
        Destroy(popup);
    }

    public void ShowPauseMenu()
    {
        UIPopup uip = new newPausePopup("How did you have time to pause?!");          //amount of deaths, progress to exit
        uip.Show();
        //UIManager.Instance.GetComponent<>
    }

    

}
