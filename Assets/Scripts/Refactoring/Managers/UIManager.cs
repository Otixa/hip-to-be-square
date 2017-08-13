using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Canvas))]
public class UIManager : Singleton<UIManager> {

    public Text playerName;         //get reference to the UI text object that displays the player name
    public Text currentScore;       //get reference to the UI text object that displays the current score
    public Text slowSpeedPoints;    //get reference to the UI text object that displays the slow speed points that remain
    public Text attemptCount;         //display how many deaths 
    public DeathMenu deathMenu;     //deathMenu is a game object with image and text and clickable button
    public Text buffDurationText;   //get reference to the text UI object that will show the current duration of buff
    public Image buffImage;         //reference to the image component that will display the larger buff type image
    private float buffDurationCounter;          //this used to display in the GUI
    public string nameText;			//variable used to store the name to display in the UI


    public GameObject speedBar;                         //reference to the UI image that represents how much time slow down points we have remaining (the blue bar)
    public GameObject pointsBar;                        //reference to the image that represents the numbers of points gained
    public Sprite[] buffImages;                         //to store the images for the buff UI#
    private GenericPlayer player;                       //reference needed so we can access character name and speed bar value
    private MapSectionGenerator theMapGen;              //reference needed so we can stop generating map when we accumulate enough points

    private UIManager() { }

    private void Awake()
    {
        GameManager2.Instance.OnPlayerDeath += OnDeath;     //subscribe to the event OnPlayerDeath, and run the OnDeath() function when invoked
    }

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<GenericPlayer>();                         //get hold of the player
        theMapGen = FindObjectOfType<MapSectionGenerator>();                //get hold of the MapGenerator
        nameText = player.playerStats.name;                                 //grab the name field from the player
        playerName.text = nameText;                                         //set the text to show that name         
        slowSpeedPoints.text = "Slow Time: " + player.playerStats.focus.ToString("F2") + " / " + player.playerStats.focus;  //display our speed points text (under the blue bar)
        buffImage.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        buffDurationText.color = new Color(1f, 1f, 1f, 0f);
    }

    // Update is called once per frame
    void Update () {
        GameManager2 gameManager = FindObjectOfType<GameManager2>();
        currentScore.text = "Score: " + gameManager.scoreCounter.ToString("F0");            //update the text on the UI to show the score
        playerName.text = nameText;                                             //update the text on the UI to show the name (this may change mid game)
        //need to ensure scoreCounter isn't 0, else you're dividing by zero.    value == 1 ? Periods.VariablePeriods : Periods.FixedPeriods
        pointsBar.transform.localScale = new Vector3(gameManager.scoreCap == 0 ? 0f : (gameManager.scoreCounter / gameManager.scoreCap), pointsBar.transform.localScale.y, pointsBar.transform.localScale.z);
        slowSpeedPoints.text = "Focus: " + Mathf.Round(player.playerStats.focus)/*.ToString("F2")*/+ " / " + player.maxFocus;       //update the text that shows how much slow time points that remain
                                                                                                                                    //adjusts the scale of the coloured portion of the HP bar. scale of 1 = full, so divide amount left by max amount to get a normalised number to pass into this
        speedBar.transform.localScale = new Vector3(player.playerStats.focus / player.maxFocus, speedBar.transform.localScale.y, speedBar.transform.localScale.z);

        //display the buff duration
        if (buffDurationCounter > 0)
        {
            buffDurationCounter -= Time.unscaledDeltaTime;
        }
        buffDurationText.text = "" + Mathf.Round(buffDurationCounter);
    }

    public void setBuff(BuffPickup buff, float duration)
    {
        if(BuffPickup.GetActive() is FocusBuff)
        {
            buffImage.GetComponent<Image>().overrideSprite = buffImages[0];
            buffImage.color = new Color(1f, 1f, 1f, 1f);
        }else if (BuffPickup.GetActive() is FocusBuff)
        {
            buffImage.GetComponent<Image>().overrideSprite = buffImages[1];
            buffImage.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            Debug.Log("Wasnt the right buff bro");
        }
        buffDurationText.color = new Color(1f, 1f, 1f, 1f);
        buffDurationCounter = duration;
    }

    public void hideBuff()
    {
        buffImage.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        buffDurationText.color = new Color(1f, 1f, 1f, 0f);
    }

    private void OnDeath(MonoBehaviour cause)
    {
        string deathMessage = "Unable to get deathmessage from event";
        deathMenu.gameObject.SetActive(true);
        if (cause is FatalObject)
        {
            deathMessage = ((FatalObject)cause).GetDeathMessage();
        }
        Debug.Log(deathMessage);
        deathMenu.GetComponentInChildren <Text>().text = deathMessage;
    }
}
