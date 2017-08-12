using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DynamicEnvironmentObject : EnvironmentObject {
    protected Rigidbody2D _rigidbody;

    protected override void Awake()
    {
        base.Awake();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    protected override void OnReset() {
        if (_rigidbody.sleepMode == RigidbodySleepMode2D.StartAsleep)
        {
            _rigidbody.Sleep();
        }
    }

    protected override void OnPlayerCollision(PlayerCollisionEvent other)
    {
        _rigidbody.WakeUp();
    }
}
