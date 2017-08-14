using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatalObject : EnvironmentObject {
    public List <String> deathMessages;
   // public string chosenMessage;
   
    protected override void OnPlayerCollision(PlayerCollisionEvent other)
    {
       // chosenMessage = GetDeathMessage();
        if (GameManager2.OnPlayerDeath != null)
        {
            GameManager2.OnPlayerDeath.Invoke(this);             //invoke the event, to be picked up by the Level Generator
        }
    }

    public string GetDeathMessage()
    {
        if(deathMessages.Count > 0)
        {
            return deathMessages[UnityEngine.Random.Range(0, deathMessages.Count)];
        }
        return "You died. And so did our death message generator.";
    }

}
