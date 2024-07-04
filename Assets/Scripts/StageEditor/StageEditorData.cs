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

    public Stage selectedStage { get; private set; }
    public Stage[] stageArray { get; private set; }
    public int selectedStageIndex { get; private set; }


    public EnemySpawnData selectedEnemySpawnData { get; private set; }
    public EnemyEditorGUI selectedEnemyEditorGUI { get; private set; }
    public int selectedEnemyIndex { get; private set; } = -1;

    public List<List<GameObject>> prefabLists { get; private set; } = new List<List<GameObject>>();
    public int prefabLength { get; private set; }

    public List<EnemySpawnData> enemySpawnDataList { get; private set; } = new List<EnemySpawnData>();
    public GameObject selectedPrefab { get; private set; }

    public Box preview { get; private set; }
    public Vector2 previewPos;

    public float cellSize = 50;

    public float inspectorLinePosX;
    public float filesLinePosX;
    public float enemyScroll;
    public float timeLength;
    public Dictionary<float, bool> timeFoldout { get; private set; } = new Dictionary<float, bool>();


    private void OnValidate()
    {
        preview = new Box(
            new Vector2(Window.GameWidth, Window.GameHeight) * cellSize,
             new Vector2(0, -0.5f * Window.GameHeight * cellSize)
            );
    }

    #region Stage

    public Stage SelectStage(int index)
    {
        if(index < 0 || stageArray.Length <= index)
        {
            selectedStage = null;
            selectedStageIndex = -1;
        }
        else
        {
            string stagePath = GetStagePath(stageArray[index]);
            selectedStage = AssetDatabase.LoadAssetAtPath<Stage>(stagePath);
            selectedStageIndex = index;

            RefreshEnemySpawnDataList();
            RefreshPrefab();
        }
        return selectedStage;
    }
    public string GetStagePath(Stage stage) => GetStagePath(stage.name);
    public string GetStagePath(string stageName) => $"{GetStageDirectoryPath(stageName)}/{stageName}.asset";

    public string GetStageDirectoryPath(Stage stage) => GetStageDirectoryPath(stage.name);
    public string GetStageDirectoryPath(string stageName) => $"Assets/Resources/Stage/{stageName}";

    public void RefreshStageArray()
    {
        string[] directoryPathArray = Directory.GetDirectories("Assets/Resources/Stage");

        stageArray = new Stage[directoryPathArray.Length];
        for (int i = 0; i < directoryPathArray.Length; i++)
        {
            string directoryPath = directoryPathArray[i];
            int lastSlashIndex = directoryPath.LastIndexOf('\\');
            string assetPath = directoryPath + directoryPath.Substring(lastSlashIndex) + ".asset";
            stageArray[i] = AssetDatabase.LoadAssetAtPath<Stage>(assetPath);
        }

        SelectStage(-1);
    }
    public void ApplyToStage()
    {
        RefreshPrefab();
        selectedStage.enemyPrefabs = new GameObject[prefabLength];
        ApplyPrefabList();

        RefreshEnemySpawnDataList();
        selectedStage.enemySpawnData = enemySpawnDataList.ToArray();
    }

    #endregion

    #region Prefab

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
    public void RefreshPrefab()
    {
        if (selectedStage == null)
        {
            prefabLength = 0;
            prefabLists.Clear();
            return;
        }
        string enemyPrefabsFolderPath = GetStageDirectoryPath(selectedStage) + "/EnemyPrefabs";
        string[] prefabListFolderPath = Directory.GetDirectories(enemyPrefabsFolderPath);
        prefabLength = 0;

        for (int folderIndex = 0; folderIndex < prefabLists.Count; folderIndex++)
        {
            prefabLists[folderIndex] = ((GameObject[])AssetDatabase.LoadAllAssetsAtPath(prefabListFolderPath[folderIndex])).ToList();
            prefabLength += prefabLists[folderIndex].Count;
        }
    }
    #endregion

    #region TimeFoldout
    
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

    #endregion

    #region EnemyEditorGUI
    
    public EnemyEditorGUI RefreshEnemyEditorGUI()
    {
        return selectedEnemyEditorGUI = StageEditor.GetEnemyEditor(selectedEnemySpawnData.EditorType);
    }

    #endregion

    #region EnemySpawnData

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
    public void RefreshEnemySpawnDataList()
    {
        if (selectedStage == null)
        {
            enemySpawnDataList.Clear();
            return;
        }
        //Assets/Resources/Stage/Stage1/EnemySpawnData
        //Assets/Resources/Stage/Stage1/EnemySpawnData
        string enemySpawnDataFolderPath = $"{GetStageDirectoryPath(selectedStage)}/EnemySpawnData";
        Debug.Log(enemySpawnDataFolderPath);
        enemySpawnDataList = ((EnemySpawnData[])AssetDatabase.LoadAllAssetsAtPath(enemySpawnDataFolderPath)).ToList();

        enemySpawnDataList.Sort((a, b) => a.spawnTime.CompareTo(b.spawnTime));
    }

    #endregion
}