using System;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public abstract class Bullet : MonoBehaviour
{
    public abstract BulletData BulletData { get; }

    public Rigidbody2D rigid;
    public Collider2D coll;

#if UNITY_EDITOR
    enum BulletType { Player, Enemy }
    [SerializeField] BulletType bulletType = BulletType.Enemy;
#endif

    Action<Bullet> updateEvent;
    Action<Bullet> destroyEvent;

    Action<Bullet, Collider2D> enterEvent;
    Action<Bullet, Collider2D> stayEvent;
    Action<Bullet, Collider2D> exitEvent;

    public void Initialize(
        Action<Bullet> updateEvent = null,
        Action<Bullet> destroyEvent = null,
        Action<Bullet, Collider2D> enterEvent = null, 
        Action<Bullet, Collider2D> stayEvent = null, 
        Action<Bullet, Collider2D> exitEvent = null
        )
    {
        this.updateEvent = updateEvent;
        this.destroyEvent = destroyEvent;

        this.enterEvent = enterEvent;
        this.stayEvent = stayEvent;
        this.exitEvent = exitEvent;

        if (rigid == null) rigid = GetComponent<Rigidbody2D>();

        if (coll == null) coll = GetComponent<CircleCollider2D>();
    }
    protected void Update()
    {
#if UNITY_EDITOR
        if (GameManager.IsEditor)
        {
            if (bulletType == BulletType.Player) gameObject.layer = LayerMask.NameToLayer("PlayerBullet");
            else if (bulletType == BulletType.Enemy) gameObject.layer = LayerMask.NameToLayer("EnemyBullet");

            if (rigid == null) rigid = GetComponent<Rigidbody2D>();
            rigid.bodyType = RigidbodyType2D.Kinematic;

            if (coll == null) coll = GetComponent<CircleCollider2D>();
            coll.isTrigger = true;
        
            return;
        }
#endif
        if (updateEvent != null) updateEvent.Invoke(this);
    }
    protected virtual void Enter(Collider2D coll)
    {
        Entity entity = coll.GetComponent<Entity>();
        entity.TakeDamage(BulletData.damage);

        Destroy();
    }
    protected virtual void Stay(Collider2D coll) { }
    protected virtual void Exit(Collider2D coll) { }

    public void Destroy()
    {
        if (destroyEvent != null) destroyEvent.Invoke(this);

        BulletData.pool.DeUse(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Enter(collision);
        if (enterEvent != null) enterEvent(this, collision);
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        Stay(collision);
        if (stayEvent != null) stayEvent(this, collision);
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        Exit(collision);
        if (exitEvent != null) exitEvent(this, collision);
    }
}
