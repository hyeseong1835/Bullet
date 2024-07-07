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
#if UNITY_EDITOR
        GameManager.instance.StartCoroutine(Editor());
#endif
        float prevTime = 0;
        for (int i = 0; i < enemySpawnData.Length; i++)
        {
            EnemySpawnData data = enemySpawnData[i];
            yield return new WaitForSeconds(data.spawnTime - prevTime);
#if UNITY_EDITOR
            StageEditor.instance.playTime = data.spawnTime;
            StageEditor.data.SelectEnemySpawnData(data);
#endif
            GameObject enemyObj = enemyPool[data.prefabIndex].Get();

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

