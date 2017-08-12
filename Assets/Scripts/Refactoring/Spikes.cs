using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : EnvironmentObject {
    public GameManager gameManager;
    [Range(-1.0f, 1.0f)] public float spikeHeightOffset = 0f;             //this is a potental way to control how far spikes petrude from the floor (for characters like chunk who struggle with default spikes)
    protected override void Awake()
    {
        base.Awake();
        gameManager = FindObjectOfType<GameManager>();
    }
    protected override void OnPlayerCollision(PlayerCollisionEvent other)
    {
        //THIS IS SHORT TERM FIX WHILE WE INTEGRATRE THE REFACTORED CODE
        gameManager.RestartGame();

        //EVENT SYSTEM WOULD BE NICER
    }

}
