using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public abstract EntityData EntityData { get; }
    public float hp;

    public virtual void Heal(float healAmount)
    {
        hp += healAmount;
        if (hp > EntityData.maxHp) hp = EntityData.maxHp;
    }
    public virtual void TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
