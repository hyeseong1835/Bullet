using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class Shooter : Weapon
{
    static Player player => Player.instance;

    [SerializeField] BulletData bulletData;

    [SerializeField] Transform look;
    [SerializeField] Transform tip;

    public Pool bulletPool = null;
    public GameObject bulletPrefab;

    [SerializeField] float chargeOnHit;
    float skillTime;

    void Awake()
    {
        bulletPool = new Pool(
                PoolType.PlayerBullet,
                bulletPrefab,
                0,
                0
            );
    }
    void Update()
    {
        if (skillTime > 0)
        {
            cooltimeMultiply = 0.25f;
            skillTime -= GameManager.deltaTime;
            if (skillTime < 0) skillTime = 0;
        }
        else
        {
            cooltimeMultiply = 1;
        }
    }
    public override void Use()
    {
        look.transform.rotation = player.input.toMouseRot;

        GameObject obj = bulletPool.Get();
        obj.transform.position = tip.position;
        obj.transform.rotation = player.input.toMouseRot;

        Bullet bullet = obj.GetComponent<Bullet>();
        bullet.data = bulletData;
        bullet.Initialize(
            BulletUpdate,
            BulletDeUse,
            BulletEnter,
            null,
            null
        );    

        bulletPool.Use(bulletPool.objects.Count - 1, obj);
    }
    void BulletUpdate(Bullet bullet)
    {
        bullet.transform.position += bullet.transform.up * bullet.data.speed * GameManager.deltaTime;

        if (bullet.coll.ToBox().IsExitGame(bullet.transform.position))
        {
            bullet.DeUse();
        }
    }
    void BulletDeUse(Bullet bullet)
    {
        bulletPool.DeUse(bullet.gameObject);
    }
    void BulletEnter(Bullet bullet, Collider2D coll)
    {
        Entity entity = coll.GetComponent<Entity>();
        entity.TakeDamage(bullet.data.damage * player.damage);

        bullet.DeUse();
        player.SkillCharge(chargeOnHit);
    }
    public override void Skill()
    {
        Player.instance.StopCoroutine("WeaponUseCoolTime");
        Player.instance.canUseWeapon = true;
        skillTime = 5;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(tip.position, tip.position + 0.1f * tip.up);
    }
}
