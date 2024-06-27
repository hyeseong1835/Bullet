using System;
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

    protected float damage;
    protected float speed;

    Action<Bullet> updateEvent;
    public Action<Bullet> destroyEvent;

    Action<Bullet, Collider2D> enterEvent;
    Action<Bullet, Collider2D> stayEvent;
    Action<Bullet, Collider2D> exitEvent;

    public void Initialize(
        float damage, float speed,
        Action<Bullet> updateEvent = null,
        Action<Bullet> destroyEvent = null,
        Action<Bullet, Collider2D> enterEvent = null, 
        Action<Bullet, Collider2D> stayEvent = null, 
        Action<Bullet, Collider2D> exitEvent = null,
        Rigidbody2D rigid = null, Collider2D coll = null
        )
    {
        this.damage = damage;
        this.speed = speed;

        this.updateEvent = updateEvent;
        this.destroyEvent = destroyEvent;

        this.enterEvent = enterEvent;
        this.stayEvent = stayEvent;
        this.exitEvent = exitEvent;

        if (rigid == null) this.rigid = GetComponent<Rigidbody2D>();
        else this.rigid = rigid;

        if (coll == null) this.coll = GetComponent<Collider2D>();
        else this.coll = coll;
    }
    protected void Update()
    {
        if (GameManager.IsEditor)
        {
            if (bulletType == BulletType.Player) gameObject.layer = LayerMask.NameToLayer("PlayerBullet");
            else if (bulletType == BulletType.Enemy) gameObject.layer = LayerMask.NameToLayer("EnemyBullet");

            Rigidbody2D rigid = GetComponent<Rigidbody2D>();
            rigid.bodyType = RigidbodyType2D.Kinematic;

            CircleCollider2D collider = GetComponent<CircleCollider2D>();
            collider.isTrigger = true;

            return;
        }
    }
    protected virtual void Enter(Collider2D coll)
    {
        Entity entity = coll.GetComponent<Entity>();
        entity.TakeDamage(damage);

        DeUse();
    }
    protected virtual void Stay(Collider2D coll) { }
    protected virtual void Exit(Collider2D coll) { }

    public virtual void DeUse()
    {
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
