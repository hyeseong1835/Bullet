using System;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
#if UNITY_EDITOR
    enum BulletType
    {
        Player,
        Enemy
    }
    [SerializeField] BulletType bulletType = BulletType.Enemy;
#endif
    Action<Bullet, Collider2D> enterEvent;
    Action<Bullet, Collider2D> stayEvent;
    Action<Bullet, Collider2D> exitEvent;

    public void Initialize(Action<Bullet, Collider2D> enter = null, Action<Bullet, Collider2D> stay = null, Action<Bullet, Collider2D> exit = null)
    {
        enterEvent = enter;
        stayEvent = stay;
        exitEvent = exit;
    }
    void Update()
    {
        if (GameManager.isEditor)
        {
            if (bulletType == BulletType.Player) gameObject.layer = LayerMask.NameToLayer("PlayerBullet");
            else if (bulletType == BulletType.Enemy) gameObject.layer = LayerMask.NameToLayer("EnemyBullet");

            Rigidbody2D rigid = GetComponent<Rigidbody2D>();
            rigid.bodyType = RigidbodyType2D.Kinematic;

            CircleCollider2D collider = GetComponent<CircleCollider2D>();
            collider.isTrigger = true;
        }
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
