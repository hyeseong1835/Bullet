using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public WeaponData data;

    public float cooltime;

    public abstract void Use();
}
