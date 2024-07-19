using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponData data;

    [Header("Stat")]
    public float cooltime;

    public abstract void Use();
}
