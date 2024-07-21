using System;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponData data;

    public float cooltime;
    [NonSerialized] public float cooltimeMultiply = 1;

    public abstract void Use();
    public abstract void Skill();
}
