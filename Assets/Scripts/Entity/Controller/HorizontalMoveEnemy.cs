using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HorizontalMoveEnemy : Enemy
{
    public HorizontalMoveEnemyData data;
    public override EnemyData EnemyData => data;

    [SerializeField] Weapon weapon;
    [SerializeField] float move;

    void Awake()
    {
        move = data.speed;
    }

    new void Update()
    {
        base.Update();

        if (GameManager.IsEditor) return;

        if (state == EntityState.Enable)
        {
            Move();
            UseWeapon();
        }
    }
    void Move()
    {
        if (transform.position.x < -0.8f) move = data.speed;
        else if (transform.position.x > 0.8f) move = -data.speed;
        
        transform.position += Vector3.right * move * Time.deltaTime;
    }
    void UseWeapon()
    {
        if (weapon == null) return;

        weapon.TryUse();
    }
}
