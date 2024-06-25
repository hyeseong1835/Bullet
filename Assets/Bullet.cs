using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    Action<Bullet, Collider2D> enterEvent;
    Action<Bullet, Collider2D> stayEvent;
    Action<Bullet, Collider2D> exitEvent;

    public void Initialize(Action<Bullet, Collider2D> enter = null, Action<Bullet, Collider2D> stay = null, Action<Bullet, Collider2D> exit = null)
    {
        enterEvent = enter;
        stayEvent = stay;
        exitEvent = exit;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (enterEvent != null) enterEvent(this, collision);
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (stayEvent != null) stayEvent(this, collision);
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (exitEvent != null) exitEvent(this, collision);
    }
}
