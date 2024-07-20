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

    Vector3 dir;

    void OnEnable()
    {
        if (spawnData == null) return;

        hp = data.maxHp;
        transform.position = spawnData.startPos;
        dir = (spawnData.endPos - spawnData.startPos).normalized;
    }
    void Start()
    {

    }
    void Update()
    {
        transform.position += dir * data.speed * GameManager.deltaTime;

        if (dir.x > 0)
        {
            if (transform.position.x > spawnData.endPos.x)
            {
                DeUse();
            }
        }
        else if (dir.x < 0)
        {
            if (transform.position.x < spawnData.endPos.x)
            {
                DeUse();
            }
        }

        if (dir.x > 0)
        {
            if (transform.position.y > spawnData.endPos.y)
            {
                DeUse();
            }
        }
        else if (dir.x < 0)
        {
            if (transform.position.y < spawnData.endPos.y)
            {
                DeUse();
            }
        }
    }
    void DeUse()
    {
        GameManager.instance.stage.enemyPool[spawnData.prefabTypeIndex][spawnData.prefabIndex].DeUse(gameObject);
    }
    protected override Vector2 GetDropDir() => dir;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 10)
        {
            other.GetComponent<Player>().TakeDamage(data.collideDamage);
        }
    }
}
