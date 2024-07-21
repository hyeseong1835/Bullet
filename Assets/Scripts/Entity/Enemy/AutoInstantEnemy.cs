using System;
using UnityEngine;

public class AutoInstantEnemy : Enemy
{
    public override EnemyData EnemyData {
        get => data; 
        set { data = (AutoInstantEnemyData)value; } 
    }

    public override Type EnemySpawnDataType => typeof(AutoInstantEnemySpawnData);
    public override EnemySpawnData EnemySpawnData { 
        get => spawnData; 
        set { spawnData = (AutoInstantEnemySpawnData)value; } 
    }

    public AutoInstantEnemyData data;
    AutoInstantEnemySpawnData spawnData;
    Vector2 dir;

    void OnEnable()
    {
        if (spawnData == null) return;

        hp = data.maxHp;
        transform.position = spawnData.startPos;
        dir = ((Vector2)Player.instance.transform.position - spawnData.startPos).normalized;
        transform.rotation = Quaternion.Euler(0, 0, -90 + Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }
    void Start()
    {

    }
    void Update()
    {
        transform.position += (Vector3)dir * data.speed * GameManager.deltaTime;
        
        if (dir.x > 0)
        {
            if(transform.position.x > Window.instance.gameRight + 1)
            {
                DeUse();
            }
        }
        else if (dir.x < 0)
        {
            if (transform.position.x < Window.instance.gameLeft - 1)
            {
                DeUse();
            }
        }

        if (dir.x > 0)
        {
            if (transform.position.y > Window.instance.gameTop + 1)
            {
                DeUse();
            }
        }
        else if (dir.x < 0)
        {
            if (transform.position.y < Window.instance.gameLeft - 1)
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
