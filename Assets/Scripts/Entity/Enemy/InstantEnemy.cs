using System;
using UnityEngine;

public class InstantEnemy : Enemy
{
    public InstantEnemyData data;
    public override EnemyData EnemyData {
        get => data; 
        set { data = (InstantEnemyData)value; } 
    }

    InstantEnemySpawnData spawnData;
    public override Type EnemySpawnDataType => typeof(InstantEnemySpawnData);
    public override EnemySpawnData EnemySpawnData { 
        get => spawnData; 
        set { spawnData = (InstantEnemySpawnData) value; } 
    }

    public Weapon weapon;

    public float speed;

    Vector3 dir;
    new void OnEnable()
    {
        if (spawnData == null) return;

        base.OnEnable();

        hp = data.maxHp;
        transform.position = spawnData.startPos;
        dir = spawnData.endPos - spawnData.startPos;
    }
    void Start()
    {

    }
    void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
        /*
        if (dir.x != 0) CheckOver(dir.x, transform.position.x, spawnData.endPos.x);
        else CheckOver(dir.y, transform.position.y, spawnData.endPos.y);

        void CheckOver(float dir, float cur, float end)
        {
            if ((dir > 0) ? cur > end : cur < end) Destroy(gameObject);
        }
        */
    }
}
