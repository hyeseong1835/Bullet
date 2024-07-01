using UnityEngine;

public class InstantEnemy : Enemy
{
    public InstantEnemyData data;
    public override EnemyData EnemyData {
        get => data; 
        set { data = (InstantEnemyData)value; } 
    }

    InstantEnemySpawnData spawnData;
    public override EnemySpawnData EnemySpawnData { 
        get => spawnData; 
        set { spawnData = (InstantEnemySpawnData) value; } 
    }

    public float speed;

    Vector3 dir;
    
    void Start()
    {
        dir = spawnData.endPos - spawnData.startPos;
    }
    void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
        if (dir.x != 0) CheckOver(dir.x, transform.position.x, spawnData.endPos.x);
        else CheckOver(dir.y, transform.position.y, spawnData.endPos.y);

        void CheckOver(float dir, float cur, float end)
        {
            if ((dir > 0) ? cur > end : cur < end) Destroy(gameObject);
        }
    }
}
