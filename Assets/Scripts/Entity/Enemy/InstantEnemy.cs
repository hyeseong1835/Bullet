using System;
using System.Runtime.Serialization.Formatters;
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

    public float speed;

    Vector3 dir;

    void OnEnable()
    {
        if (spawnData == null) return;

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
        
        if (spawnData.endPos.x < spawnData.startPos.x)
        {
            if(transform.position.x < spawnData.endPos.x)
            {
                DeUse();
            }
        }
        else if (spawnData.endPos.x > spawnData.startPos.x)
        {
            if (transform.position.x > spawnData.endPos.x)
            {
                DeUse();
            }
        }

        if (spawnData.endPos.y < spawnData.startPos.y)
        {
            if (transform.position.y < spawnData.endPos.y)
            {
                DeUse();
            }
        }
        else if (spawnData.endPos.y > spawnData.startPos.y)
        {
            if (transform.position.y > spawnData.endPos.y)
            {
                DeUse();
            }
        }

        /*
        if (dir.x != 0) CheckOver(dir.x, transform.position.x, spawnData.endPos.x);
        else CheckOver(dir.y, transform.position.y, spawnData.endPos.y);

        void CheckOver(float dir, float cur, float end)
        {
            if ((dir > 0) ? cur > end : cur < end) Destroy(gameObject);
        }
        */
    }
    void DeUse()
    {
        GameManager.instance.stage.enemyPool[spawnData.prefabTypeIndex][spawnData.prefabIndex].DeUse(gameObject);
    }
}
