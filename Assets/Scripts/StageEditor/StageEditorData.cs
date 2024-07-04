using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "Data", menuName = "StageEditor/Data")]
public class StageEditorData : ScriptableObject
{
    public struct EditorEnemyData
    {
        public Vector2 worldPos;
        public Vector2 definition;
        public float spawnTime;

        public EditorEnemyData(Vector2 worldPos, Vector2 definition, float spawnTime)
        {
            this.worldPos = worldPos;

            this.definition = definition;

            this.spawnTime = spawnTime;
        }
    }

    public Stage selectedStage;
    public string selectedStageFolderResourcePath;
    public int selectedStageIndex;
    public string[] stageNameArray;

    public EnemySpawnData selectedEnemySpawnData { get; private set; }
    public EnemyEditorGUI selectedEnemyEditorGUI { get; private set; }
    public int selectedEnemyIndex = -1;

    public List<List<GameObject>> prefabLists = new List<List<GameObject>>();
    public int prefabLength;

    public List<EnemySpawnData> enemySpawnDataList = new List<EnemySpawnData>();
    public GameObject selectedPrefab { get; private set; }

    public Box preview;
    public Vector2 previewPos;

    public float cellSize = 50;

    public float inspectorLinePosX;
    public float filesLinePosX;
    public float enemyScroll = 0;
    public float timeLength = 60;
    public Dictionary<float, bool> timeFoldout = new Dictionary<float, bool>();
    public bool debug = false;


    private void OnValidate()
    {
        preview = new Box();
        preview.size = new Vector2(Window.GameWidth, Window.GameHeight) * cellSize;
        preview.center = new Vector2(0, -0.5f * Window.GameHeight * cellSize);
    }
    public List<GameObject> GetPrefabList(GameObject obj)
    {
        return GetPrefabList(obj.GetComponent<Enemy>().GetType());
    }
    public List<GameObject> GetPrefabList(Type type)
    {
        for (int listIndex = prefabLists.Count; listIndex >= 0; listIndex--)
        {
            List<GameObject> curList = prefabLists[listIndex];

            if (curList.Count <= 0)
            {
                prefabLists.RemoveAt(listIndex);
                continue;
            }
            Type listType = curList[0].GetType();

            if(listType == type) return curList;
        }
        prefabLists.Add(new List<GameObject>());
        return prefabLists[^1];
    }
    public void ApplyPrefabList()
    {
        int i = 0;
        for (int listIndex = 0; listIndex < prefabLists.Count; listIndex++)
        {
            List<GameObject> curList = prefabLists[listIndex];
            for (int elementIndex = 0; elementIndex < curList.Count; elementIndex++)
            {
                selectedStage.enemyPrefabs[i] = curList[elementIndex];
                i++;
            }
        }
    }
    public void ApplyToStage()
    {
        RefreshPrefab();
        selectedStage.enemyPrefabs = new GameObject[prefabLength];
        ApplyPrefabList();

        RefreshStagePath();
        RefreshEnemySpawnDataList();
        selectedStage.enemySpawnData = enemySpawnDataList.ToArray();
    }
    
    public void RefreshTimeFoldout()
    {
        float prevTime = -1;

        Dictionary<float, bool> newTimeFoldout = new Dictionary<float, bool>();
        foreach (EnemySpawnData enemyData in enemySpawnDataList)
        {
            if (enemyData.spawnTime != prevTime)
            {
                if (timeFoldout.TryGetValue(enemyData.spawnTime, out bool foldout))
                {
                    if (newTimeFoldout.ContainsKey(enemyData.spawnTime) == false)
                    {
                        newTimeFoldout.Add(enemyData.spawnTime, foldout);
                    }
                }
                else
                {
                    newTimeFoldout.Add(enemyData.spawnTime, false);
                }
            }
        }
        timeFoldout = newTimeFoldout;
    }
    public void ResetTimeFoldout()
    {
        float prevTime = -1;

        timeFoldout = new Dictionary<float, bool>();
        foreach (EnemySpawnData enemyData in enemySpawnDataList)
        {
            if (enemyData.spawnTime != prevTime)
            {
                if (timeFoldout.ContainsKey(enemyData.spawnTime))
                {
                    timeFoldout[enemyData.spawnTime] = false;
                }
                else timeFoldout.Add(enemyData.spawnTime, false);
            }
        }
    }
    public EnemySpawnData SelectEnemySpawnData(int index)
    {
        selectedEnemyIndex = index;

        if (index == -1)
        {
            selectedEnemySpawnData = null;
            selectedPrefab = null;
            selectedEnemyEditorGUI = null;
        }
        else
        {
            selectedEnemySpawnData = enemySpawnDataList[index];
            
            if (selectedEnemySpawnData.prefabIndex < 0 || selectedStage.enemyPrefabs.Length <= selectedEnemySpawnData.prefabIndex)
            {
                selectedEnemySpawnData.prefabIndex = -1;
                selectedPrefab = null;
            } 
            else selectedPrefab = selectedStage.enemyPrefabs[selectedEnemySpawnData.prefabIndex];

            selectedEnemyEditorGUI = StageEditor.GetEnemyEditor(selectedEnemySpawnData.EditorType);
        }
        return selectedEnemySpawnData;
    }
    public EnemyEditorGUI RefreshEnemyEditorGUI()
    {
        return selectedEnemyEditorGUI = StageEditor.GetEnemyEditor(selectedEnemySpawnData.EditorType);
    }

    public int RemoveAndSortInEnemySpawnDataList(EnemySpawnData spawnData, int index)
    {
        enemySpawnDataList.RemoveAt(index);

        return InsertToEnemySpawnDataList(spawnData);
    }
    public int InsertToEnemySpawnDataList(EnemySpawnData spawnData)
    {
        int index = enemySpawnDataList.TakeWhile(x => (x.spawnTime < spawnData.spawnTime)).Count();
        enemySpawnDataList.Insert(index, spawnData);

        return index;
    }
    public void RefreshStagePath()
    {
        string stageDataPath = AssetDatabase.GetAssetPath(selectedStage);
        string stageDataResourcePath = stageDataPath.Substring("Assets/Resources/".Length);
        selectedStageFolderResourcePath = stageDataResourcePath.Substring(0, stageDataResourcePath.Length - (selectedStage.name.Length + ".assets".Length));
    }
    public void RefreshEnemySpawnDataList()
    {
        string[] enemySpawnDataPathArray = Directory.GetFiles($"Assets/Resources/{selectedStageFolderResourcePath}/EnemySpawnData", "*.asset", SearchOption.AllDirectories);
        enemySpawnDataList.Clear();
        foreach (string path in enemySpawnDataPathArray)
        {
            EnemySpawnData data = AssetDatabase.LoadAssetAtPath<EnemySpawnData>(path);
            enemySpawnDataList.Add(data);
        }
        enemySpawnDataList.Sort((a, b) => a.spawnTime.CompareTo(b.spawnTime));
    }
    public void RefreshPrefab()
    {
        string[] directoryPath = Directory.GetDirectories($"Assets/Resources/{selectedStageFolderResourcePath}/EnemyPrefabs");
        prefabLength = 0;

        for (int directoryPathIndex = 0; directoryPathIndex < prefabLists.Count; directoryPathIndex++)
        {
            prefabLists[directoryPathIndex] = ((GameObject[])AssetDatabase.LoadAllAssetsAtPath(directoryPath[directoryPathIndex])).ToList();
            prefabLength += prefabLists[directoryPathIndex].Count;
        }
    }
}