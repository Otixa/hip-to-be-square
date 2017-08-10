using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResettableObject : MonoBehaviour {
    /// <summary>
    /// The original start position of the Object when firs
    /// </summary>
    public Vector3 resetPosition;                   //as will need to access this from object pooler when positions are changing as objects are spawned and destroyed
    public bool shouldReset = true;                 //for special situation where we want a resettableObject to behave differently 

    //this will be set 
    protected virtual void OnReset() { }

    // Use this for initialization
    private void Start () {         //First enabled                             (//Awake when initialised, regardless of being enabled or not)
        resetPosition = transform.position;
	}

    private void OnEnable()
    {
        //Set current position to be the stored startingPosition (which is when the object is instantiated
        if (shouldReset && resetPosition != transform.position)
        {
            transform.position = resetPosition;
            OnReset();
        }
        resetPosition = transform.position;
    }

    

}
