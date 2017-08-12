using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusPickup : Pickup
{
    [SerializeField] private int amountToGive = 1;
    private GenericPlayer thePlayer;

    protected override void Awake()
    {
        base.Awake();
        thePlayer = FindObjectOfType<GenericPlayer>();       //we need this reference to adjust the focus points
    }

    public override void OnPickup(PlayerCollisionEvent other)   //updates the statistics relating to the pickup, passing to it the amount of focus this pickup is worth
    {   
        thePlayer.playerStats.focus = Mathf.Clamp(thePlayer.playerStats.focus + amountToGive, 0, thePlayer.maxFocus);   
    }
}
