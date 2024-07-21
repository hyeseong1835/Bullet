using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1Boss : Entity
{
    public DefaultEntity rightShooter;
    public DefaultEntity leftShooter;

    public float bodyMaxHp;
    public override float GetMaxHP() => bodyMaxHp;
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
    }
    void OnLeftShooterDamaged(float damage)
    {
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

        hp -= trueDamage;
        if (hp < 0) hp = 0;

        onDamaged?.Invoke(trueDamage);

        if (hp == 0)
        {
            OnDead();
        }
    }
    protected override void OnDead()
    {
        GameManager.instance.GameWin();
        Destroy(gameObject);
    }
}