using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GridBrushBase;

[CreateAssetMenu(fileName = "Stage", menuName = "Data/Stage")]
public class Stage : ScriptableObject
{
    public string StageDirectoryResourcePath => $"Stage/{name}";
    public string[] enemyPrefabFolderNameArray;
    
    public StageEvent[] stageEvent;
    
    public float timeLength = 0;


    public EnemySpawnData[] enemySpawnDataArray { get; private set; }
    public Pool[][] enemyPool { get; private set; }
    public int lastIndex { get; set; } = -1;

#if UNITY_EDITOR
    void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnEditorPlayModeStateChanged;
    }
    void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnEditorPlayModeStateChanged;
    }
    void OnEditorPlayModeStateChanged(PlayModeStateChange state)
    {
        switch (state)
        {
            case PlayModeStateChange.ExitingPlayMode:
                enemyPool = null;
                break;
        }
    }
#endif
    public void Init()
    {
        List<EnemySpawnData> enemySpawnDataList = new List<EnemySpawnData>();
        List<Pool[]> poolLists = new List<Pool[]>();
        
        for (int folderIndex = 0; folderIndex < enemyPrefabFolderNameArray.Length; folderIndex++)
        {
            string folderName = enemyPrefabFolderNameArray[folderIndex];
            GameObject[] enemyPrefabArray = Resources.LoadAll<GameObject>($"{StageDirectoryResourcePath}/EnemyPrefabs/{folderName}");
            
            // Enemy Spawn Data
            string enemySpawnDataFolderResourcePath = $"{StageDirectoryResourcePath}/EnemySpawnData/{folderName}";
            EnemySpawnData[] enemySpawnDataArray = Resources.LoadAll<EnemySpawnData>(enemySpawnDataFolderResourcePath);
            enemySpawnDataList.AddRange(enemySpawnDataArray);

            // Pool
            Pool[] poolArray = new Pool[enemyPrefabArray.Length];

            for (int poolIndex = 0; poolIndex < enemyPrefabArray.Length; poolIndex++)
            {
                Pool pool = new Pool(
                    PoolType.Enemy,
                    enemyPrefabArray[poolIndex], 
                    0, 
                    0
                );
                poolArray[poolIndex] = pool;
            }
            poolLists.Add(poolArray);
        }
        // Enemy Spawn Data
        enemySpawnDataList.Sort((a, b) => a.spawnTime.CompareTo(b.spawnTime));
        enemySpawnDataArray = enemySpawnDataList.ToArray();

        // Pool
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
    
    public void Read(int startIndex, ref int eventIndex, float curTime)
    {
        for (int i = eventIndex; i < stageEvent.Length; i++)
        {
            StageEvent e = stageEvent[i];
            
            if (curTime < e.time) break;

            eventIndex = i + 1;
            e.Invoke();
        }

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
            Pool pool = enemyPool[data.prefabTypeIndex][data.prefabIndex];
            GameObject enemyObj = pool.Get();

            Enemy enemy = enemyObj.GetComponent<Enemy>();
            enemy.EnemySpawnData = data;
            pool.Use();
        }
    }
}

