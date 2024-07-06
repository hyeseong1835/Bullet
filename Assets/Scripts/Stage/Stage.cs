using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "Data/Stage")]
public class Stage : ScriptableObject
{
    public EnemySpawnData[] enemySpawnData;
    public GameObject[] enemyPrefabs;
    public Pool[] enemyPool;

    public void Init()
    {
        enemyPool = new Pool[enemyPrefabs.Length];
        
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            GameObject prefab = enemyPrefabs[i];
            Pool pool = new Pool(prefab, 1, 0, 0);
            pool.Init();
            enemyPool[i] = pool;
        }
    }
    public IEnumerator Start()
    {
        float prevTime = 0;
        for (int i = 0; i < enemySpawnData.Length; i++)
        {
            EnemySpawnData data = enemySpawnData[i];
            yield return new WaitForSeconds(data.spawnTime - prevTime);
            GameObject enemyObj = enemyPool[data.prefabIndex].Get();

            Enemy enemy = enemyObj.GetComponent<Enemy>();
            enemy.EnemySpawnData = data;
            enemyObj.SetActive(true);
            prevTime = data.spawnTime;
            
            Debug.Log($"Spawn: {enemyObj.name} [{data.spawnTime}]");
        }
    }
}

