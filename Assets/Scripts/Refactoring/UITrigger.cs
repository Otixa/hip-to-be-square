using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITrigger : PersistentPlayerTrigger {
    public string theMessage = "I am a default UI Message";         //this should be set via inspector
    public GameObject popupType;                                    //this will have to be set via the inspector

    public override void WhenTriggered(PlayerCollisionEvent other)
    {
        UIManager.Instance.LoadPopup(popupType, theMessage/*, CALLBACK HERER*/);
    }

}
