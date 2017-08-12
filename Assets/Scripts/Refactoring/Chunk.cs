using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : GenericPlayer {

    public Chunk()
    {
        moveVelocity = 3f;
        jumpVelocity = 6f;
        jumpDuration = 0.4f;

        _rigidbody.mass = 5f;

        //_collider.offset.Set(0f, 0.375f);
    }
}
