using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public abstract WeaponData WeaponData { get; }
    public bool canUse = true;

    public virtual bool TryUse()
    {
        if (canUse)
        {
            StartCoroutine(CoolTime(WeaponData.cooltime));
            Use();
            return true;
        }
        return false;
    }
    IEnumerator CoolTime(float time)
    {
        canUse = false;

        yield return new WaitForSeconds(time);

        canUse = true;
    }
    protected abstract void Use();
}
