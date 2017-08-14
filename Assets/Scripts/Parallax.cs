using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {
    public float speed = 0.36f;
    //public CameraController cc;
    public GenericPlayer gp;

    void Start() {
        //cc = FindObjectOfType<CameraController>();
        gp = FindObjectOfType<GenericPlayer>();
    }

    void Update() {
        //we basically want a 
        //float newSpeed = cc.distanceMoved;          //if you havent moved, distance moved will be zero, therefore speed will be zero.
        //Vector2 offset = new Vector2(Time.time * newSpeed, 0);
        //Vector2 offset = new Vector2(Time.time * speed, 0);
        Vector2 offset = new Vector2(gp.transform.position.x * speed, 0);

        //if(cc.distanceMoved > 0) { 
        //    Vector2 offset = new Vector2(Time.time * (cc.distanceMoved*1), 0);
        GetComponent<Renderer>().material.mainTextureOffset = offset;
        //}
        //speed += 0.000000666f;
        
    }

}

