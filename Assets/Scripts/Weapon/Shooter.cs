using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Shooter : Weapon
{
    public ShooterData data;
    public override WeaponData WeaponData => data;
    
    [SerializeField] Transform tip;
    Bullet bullet;

    void Awake()
    {
        bullet = data.bulletPrefab.GetComponent<Bullet>();
        
        if (bullet.BulletData.pool.holder == null) bullet.BulletData.pool.Init();
    }
    protected override void Use()
    {
        GameObject obj = bullet.BulletData.pool.Use();
        obj.transform.position = tip.position;
        obj.transform.rotation = tip.rotation;
        obj.GetComponent<Bullet>().Initialize();
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(tip.position, tip.position + 0.1f * tip.up);
    }
}
