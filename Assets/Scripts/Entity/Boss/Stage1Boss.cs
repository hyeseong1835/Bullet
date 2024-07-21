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
    protected override void OnDead()
    {
        GameManager.instance.GameWin();
        Destroy(gameObject);
    }
}