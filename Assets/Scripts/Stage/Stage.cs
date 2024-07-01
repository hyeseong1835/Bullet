using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "Data/Stage")]
public class Stage : ScriptableObject
{
    public string stageName;
    public EnemySpawnData[] enemySpawnData;
    //public Pool[] enemyPool;

    public IEnumerator Start()
    {
        float prevTime = 0;
        for (int i = 0; i < enemySpawnData.Length; i++)
        {
            EnemySpawnData data = enemySpawnData[i];

            yield return new WaitForSeconds(data.spawnTime - prevTime);

            GameObject enemyObj = Instantiate(data.enemyPrefab);

            prevTime = data.spawnTime;
        }
    }
}

