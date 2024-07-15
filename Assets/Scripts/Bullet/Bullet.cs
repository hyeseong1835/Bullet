using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    public BulletData data;

    [HideInInspector] public Rigidbody2D rigid;
    [HideInInspector] public Collider2D coll;

    Action<Bullet> updateEvent;
    Action<Bullet> deUseEvent;

    Action<Bullet, Collider2D> enterEvent;
    Action<Bullet, Collider2D> stayEvent;
    Action<Bullet, Collider2D> exitEvent;

    public void Initialize(
        Action<Bullet> updateEvent = null,
        Action<Bullet> deUseEvent = null,
        Action<Bullet, Collider2D> enterEvent = null, 
        Action<Bullet, Collider2D> stayEvent = null, 
        Action<Bullet, Collider2D> exitEvent = null
        )
    {
        this.updateEvent = updateEvent;
        this.deUseEvent = deUseEvent;

        this.enterEvent = enterEvent;
        this.stayEvent = stayEvent;
        this.exitEvent = exitEvent;

        if (rigid == null) rigid = GetComponent<Rigidbody2D>();

        if (coll == null) coll = GetComponent<CircleCollider2D>();
    }
    protected void Update()
    {
        if (updateEvent != null) updateEvent.Invoke(this);
    }
    public void DeUse()
    {
        if (deUseEvent != null) deUseEvent.Invoke(this);
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
