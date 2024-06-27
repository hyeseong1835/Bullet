using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : Weapon
{
    [SerializeField] Pool bulletPool;
    [SerializeField] float bulletSpeed;
    [SerializeField] Transform tip;

    void Start()
    {
        bulletPool.Init(BulletPoolInit);
    }
    void BulletPoolInit(GameObject obj)
    {
        obj.GetComponent<Bullet>().Initialize(bulletSpeed, delete: BulletDeUse, enter: BulletEnter);
    }
    protected override void Use()
    {
        bulletPool.Use().transform.position = tip.position;
    }

    void BulletDeUse(Bullet bullet)
    {
        bulletPool.DeUse(bullet.gameObject);
    }
    void BulletEnter(Bullet bullet, Collider2D coll)
    {
        Entity entity = coll.GetComponent<Entity>();
        entity.TakeDamage(damage);

        BulletDeUse(bullet);
    }
}
