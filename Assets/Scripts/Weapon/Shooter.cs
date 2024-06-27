using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Shooter : Weapon
{
    public ShooterData data;
    public override WeaponData WeaponData => data;

    [SerializeField] float damage;
    [SerializeField] float speed;

    protected override void Use()
    {
        GameObject obj = Instantiate(data.bulletPrefab);
        obj.transform.position = data.tip.position;
        obj.transform.rotation = data.tip.rotation;
        obj.GetComponent<Bullet>().Initialize(damage, speed);
    }
}
