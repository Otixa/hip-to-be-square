using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : ResettableObject {
    public float width = 8f;                    //how wide the platform is
    public float entryPoint = 0f;               //the y co-ordinate of the point the player starts on the platform, in relation to the anchor of the prefab (0 should be fine)
    public float exitPoint = 0f;                //the y co-ordinate of the point where the player leaves the platform, in relation to the anchor of the prefab.
    public bool canSpawnCoins = false;          //whether this platform is suitable to have the spawnCoins algorithm applied to it
    public bool canSpawnSpikes = false;         //whether this platform is suitable to have the spikeSpawn algorithm applied to it
    public string forWho;                       //specifies who this platform is designed for, can hold values Joe, Whitey, Chunk, All. !!NEED DATA TYPE THAT RESTRICTS ONLY THESE 4 VALUES

    //should we create a method that makes a list of all its children elements, so that we can manually call reset on them when the platfrom object is disabled?

}
