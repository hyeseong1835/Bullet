using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "Data/Stage")]
public class Stage : ScriptableObject
{
    public GameObject[] enemyPrefabs;
    public int[] enemyPrefabArrayCounts;

    public EnemySpawnData[] enemySpawnDataArray;

    public Pool[][] enemyPool;

    public int lastIndex = -1;
    public float timeLength = 0;

    public void Init()
    {
        List<Pool[]> poolLists = new List<Pool[]>();
        int startIndex = 0;
        for (int arrayIndex = 0; arrayIndex < enemyPrefabArrayCounts.Length; arrayIndex++)
        {
            int count = enemyPrefabArrayCounts[arrayIndex];
            Pool[] poolArray = new Pool[count];

            for (int poolIndex = 0; poolIndex < count; poolIndex++)
            {
                Pool pool = new Pool(enemyPrefabs[startIndex + poolIndex], 1, 0, 0);
                pool.Init();
                poolArray[poolIndex] = pool;
            }
            poolLists.Add(poolArray);
            startIndex += count;
        }
        enemyPool = poolLists.ToArray();
    }

    /// <summary>
    /// {time} 바로 직전의 적들의 첫 번째 인덱스를 {startIndex}부터 양의 방향으로 탐색합니다.
    /// </summary>
    public int FindIndex(float time, int startIndex = 0)
    {
        int result = 0;
        float prevTime = 0;

        for (int i = startIndex; i < enemySpawnDataArray.Length; i++)
        {
            EnemySpawnData data = enemySpawnDataArray[i];
            if (data.spawnTime != prevTime)
            {
                if (time >= data.spawnTime)
                {
                    return result;
                }
                result = i;
            }
        }
        return -1;
    }
    /// <summary>
    /// {time} 바로 직전의 적들의 첫 번째 인덱스를 {startIndex}부터 음의 방향으로 탐색합니다.
    /// </summary>
    /// <param name="startIndex">-1: enemySpawnData.Length - 1</param>
    /// <returns></returns>
    public int FindBackIndex(float time, int startIndex = -1)
    {
        if (startIndex == -1) startIndex = enemySpawnDataArray.Length - 1;
        
        EnemySpawnData data = enemySpawnDataArray[startIndex];
        if (time < data.spawnTime)
        {
            return startIndex;
        }

        for (int i = startIndex - 1; i >= 0; i--)
        {
            data = enemySpawnDataArray[i];
            if (time < data.spawnTime)
            {
                return i + 1;
            }
        }
        return -1;
    }
    
    public void Read(int startIndex, float curTime)
    {
#if UNITY_EDITOR
        StageEditor.data?.RefreshStageArray();
        StageEditor.data?.SelectStage(this);
        StageEditor.instance?.Repaint();
#endif
        for (int i = startIndex; i < enemySpawnDataArray.Length; i++)
        {
            EnemySpawnData data = enemySpawnDataArray[i];

            if (data.spawnTime > curTime)
            {
                lastIndex = i - 1;
                break;
            }

#if UNITY_EDITOR
            StageEditor.data?.SelectEnemyData(data);
#endif
            GameObject enemyObj = enemyPool[data.prefabTypeIndex][data.prefabIndex].Get();

            Enemy enemy = enemyObj.GetComponent<Enemy>();
            enemy.EnemySpawnData = data;
            enemyObj.SetActive(true);
        }
    }
}

