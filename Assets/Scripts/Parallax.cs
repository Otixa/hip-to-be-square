using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {
    public float speed = 0.36f;

    void Start() { }

    void Update() {
        Vector2 offset = new Vector2(Time.time * speed, 0);
        //speed += 0.000000666f;
        GetComponent<Renderer>().material.mainTextureOffset = offset;
    }

}

