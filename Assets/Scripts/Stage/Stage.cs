using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "Data/Stage")]
public class Stage : ScriptableObject
{
    public GameObject[] enemyPrefabs;
    public int[] enemyPrefabArrayCounts;
    public EnemySpawnData[] enemySpawnData;

    public Pool[][] enemyPool;

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
    public IEnumerator Start()
    {
#if UNITY_EDITOR
        GameManager.instance.StartCoroutine(Editor());
#endif
        float prevTime = 0;
        for (int i = 0; i < enemySpawnData.Length; i++)
        {
            EnemySpawnData data = enemySpawnData[i];

            float waitTime = data.spawnTime - prevTime;
            if (waitTime != 0) yield return new WaitForSeconds(waitTime);

#if UNITY_EDITOR
            StageEditor.instance.playTime = data.spawnTime;
            StageEditor.data.SelectEnemyData(data);
#endif
            
            GameObject enemyObj = enemyPool[data.prefabTypeIndex][data.prefabIndex].Get();

            Enemy enemy = enemyObj.GetComponent<Enemy>();
            enemy.EnemySpawnData = data;
            enemyObj.SetActive(true);
            prevTime = data.spawnTime;
        }
    }
#if UNITY_EDITOR
    public IEnumerator Editor()
    {
        if (StageEditor.instance == null) StageEditor.CreateWindow();

        StageEditor.data.RefreshStageArray();
        StageEditor.data.SelectStage(this);
        StageEditor.instance.playTime = 0;

        while (StageEditor.instance.playTime < StageEditor.data.timeLength)
        {
            StageEditor.instance.playTime += Time.deltaTime;

            StageEditor.instance.Repaint();
            yield return null;
        }
    }
#endif
}

