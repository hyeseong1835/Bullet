using System;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public abstract float GetMaxHP();
    public float hp;
    public float resistance = 1;
    public Action onDead;
    public Action<float> onDamaged;

    public virtual void Heal(float healAmount)
    {
        hp += healAmount;
        if (hp > GetMaxHP()) hp = GetMaxHP();
    }
    public virtual void TakeDamage(float damage)
    {
        float trueDamage = damage * resistance;
        
        hp -= trueDamage;
        if (hp < 0) hp = 0;

        onDamaged?.Invoke(trueDamage);

        if (hp == 0)
        {
            OnDead();
        }
    }
    protected virtual void OnDead()
    {
        onDead?.Invoke();
    }
}
