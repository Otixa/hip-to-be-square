using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newUITrigger : PersistentPlayerTrigger
{
    public string theMessage = "I am a default UI Message";         //this should be set via inspector

    public override void WhenTriggered(PlayerCollisionEvent other)
    {
        UIPopup uip = new newHelpPopup(theMessage);
        uip.Show();
    }

}
