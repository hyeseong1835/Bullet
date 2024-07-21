using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1Boss : Entity
{
    public DefaultEntity rightShooter;
    public DefaultEntity leftShooter;

    public float bodyHp;
    public float bodyMaxHp;
    public override float GetMaxHP() => rightShooter.maxHp + leftShooter.maxHp + bodyMaxHp;
    void OnEnable()
    {
        rightShooter.onDamaged += OnRightShooterDamaged;
        rightShooter.onDead += OnRightShooterDead;

        leftShooter.onDamaged += OnLeftShooterDamaged;
        leftShooter.onDead += OnLeftShooterDead;
    }
    void OnDisable()
    {
        if(rightShooter != null)
        {
            rightShooter.onDamaged -= OnRightShooterDamaged;
            rightShooter.onDead -= OnRightShooterDead;
        }
        if(leftShooter != null)
        {
            leftShooter.onDamaged -= OnLeftShooterDamaged;
            leftShooter.onDead -= OnLeftShooterDead;
        }
    }
    void OnRightShooterDamaged(float damage)
    {
        Debug.Log($"RightShooter Damaged: {rightShooter.hp} (-{damage})");
        hp = bodyHp + leftShooter.hp + rightShooter.hp;
    }
    void OnLeftShooterDamaged(float damage)
    {
        Debug.Log($"LeftShooter Damaged: {leftShooter.hp} (-{damage})");
        hp = bodyHp + leftShooter.hp + rightShooter.hp;
    }

    void OnRightShooterDead()
    {
        rightShooter.gameObject.SetActive(false);
    }
    void OnLeftShooterDead()
    {
        leftShooter.gameObject.SetActive(false);
    }
    public override void TakeDamage(float damage)
    {
        float trueDamage = damage * resistance;

        bodyHp -= trueDamage;
        if (bodyHp < 0) bodyHp = 0;

        onDamaged?.Invoke(trueDamage);

        if (bodyHp == 0)
        {
            OnDead();
        }
        hp = bodyHp + rightShooter.hp + leftShooter.hp;
    }
    protected override void OnDead()
    {
        Destroy(gameObject);
    }
}