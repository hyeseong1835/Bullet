using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public abstract float GetMaxHP();
    public float hp;
    public float resistance = 1;

    public virtual void Heal(float healAmount)
    {
        hp += healAmount;
        if (hp > GetMaxHP()) hp = GetMaxHP();
    }
    public virtual void TakeDamage(float damage)
    {
        hp -= damage * resistance;
        if (hp <= 0)
        {
            OnDead();
        }
    }
    protected virtual void OnDead()
    {
        Destroy(gameObject);
    }
}
