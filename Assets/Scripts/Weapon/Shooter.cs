using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Shooter : Weapon
{
    static PlayerController player => PlayerController.instance;
    public ShooterData data;
    public override WeaponData WeaponData => data;

    [SerializeField] Transform look;
    [SerializeField] Transform tip;
    Bullet bullet;

    void Awake()
    {
        bullet = data.bulletPrefab.GetComponent<Bullet>();
        
        if (bullet.BulletData.pool.holder == null) bullet.BulletData.pool.Init();
    }
    protected override void Use()
    {
        look.transform.rotation = player.toMouseRot;

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
