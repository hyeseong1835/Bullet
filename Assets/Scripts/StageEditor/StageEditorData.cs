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
    //Stage
    public Stage selectedStage { get; private set; }
    public Stage[] stageArray { get; private set; }
    public int selectedStageIndex { get; private set; }

    //EnemySpawnData
    public EnemySpawnData selectedEnemySpawnData { get; private set; }
    public EnemyEditorGUI selectedEnemyEditorGUI { get; private set; }
    public int selectedEnemySpawnDataIndex { get; private set; } = -1;

    public List<EnemySpawnData> enemySpawnDataList { get; private set; } = new List<EnemySpawnData>();

    //Prefab
    public GameObject selectedPrefab { get; private set; }
    
    public List<List<GameObject>> prefabLists { get; private set; } = new List<List<GameObject>>();
    public List<Type> prefabTypeList { get; private set; } = new List<Type>();
    
    public int prefabLength { get; private set; }


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
        }

        RefreshEnemySpawnDataList();
        RefreshPrefabList();
        
        return selectedStage;
    }
    public string GetStagePath(Stage stage) => GetStagePath(stage.name);
    public string GetStagePath(string stageName) => $"{GetStageDirectoryPath(stageName)}/{stageName}.asset";

    public string GetStageDirectoryPath(Stage stage) => GetStageDirectoryPath(stage.name);
    public string GetStageDirectoryPath(string stageName) => $"Assets/Resources/Stage/{stageName}";

    public void RefreshStageArray()
    {
        int prevStageIndex = selectedStageIndex;
        string[] directoryPathArray = Directory.GetDirectories("Assets/Resources/Stage");

        stageArray = new Stage[directoryPathArray.Length];
        for (int i = 0; i < directoryPathArray.Length; i++)
        {
            string directoryPath = directoryPathArray[i];
            int lastSlashIndex = directoryPath.LastIndexOf('\\');
            string assetPath = directoryPath + directoryPath.Substring(lastSlashIndex) + ".asset";
            stageArray[i] = AssetDatabase.LoadAssetAtPath<Stage>(assetPath);
        }
        if (prevStageIndex < 0 || stageArray.Length <= prevStageIndex)
        {
            SelectStage(-1);
        }
        else SelectStage(prevStageIndex);
    }
    public void ApplyToStage()
    {
        RefreshPrefabList();
        selectedStage.enemyPrefabs = new GameObject[prefabLength];
        ApplyPrefabList();

        RefreshEnemySpawnDataList();
        selectedStage.enemySpawnData = enemySpawnDataList.ToArray();
    }

    #endregion

    #region Prefab

    public void SelectPrefab(int index)
    {
        if (index < 0 || selectedEnemySpawnData == null)
        {
            UnSelect();
            return;
        }
        
        List<GameObject> prefabList = GetPrefabList(selectedEnemySpawnData.GetType());
        if(prefabList.Count <= index)
        {
            UnSelect();
            return;
        }
        else
        {
            selectedPrefab = prefabList[index];
            selectedEnemySpawnData.prefabIndex = index;
            return;
        }
        void UnSelect()
        {
            selectedPrefab = null;
            selectedEnemySpawnData.prefabIndex = -1;
        }
    }
    public List<GameObject> GetAllPrefabList()
    {
        List<GameObject> result = new List<GameObject>();
        foreach (List<GameObject> list in prefabLists)
        {
            result.AddRange(list);
        }
        return result;
    }
    public List<GameObject> GetPrefabList(EnemySpawnData enemyData) => GetPrefabList(enemyData.GetType());
    public List<GameObject> GetPrefabList(Type type)
    {
        int index = GetPrefabListIndex(type);

        if (index == -1) return null;
        else return prefabLists[index];
    }
    public int GetPrefabListIndex(EnemySpawnData enemyData) => GetPrefabListIndex(enemyData.GetType());
    public int GetPrefabListIndex(Type type) => prefabTypeList.IndexOf(type);
    public void ApplyPrefabList()
    {
        selectedStage.enemyPrefabs = GetAllPrefabList().ToArray();
    }
    public void RefreshPrefabList()
    {
        prefabLength = 0;
        prefabLists.Clear();
        prefabTypeList.Clear();

        if (selectedStage == null) return;

        string enemyPrefabsFolderPath = GetStageDirectoryPath(selectedStage) + "/EnemyPrefabs";
        string[] prefabListFolderPath = Directory.GetDirectories(enemyPrefabsFolderPath);

        for (int folderIndex = 0; folderIndex < prefabListFolderPath.Length; folderIndex++)
        {
            List<GameObject> prefabList = new List<GameObject>();

            string[] prefabPathArray = Directory.GetFiles(prefabListFolderPath[folderIndex], "*.prefab");
            foreach (string path in prefabPathArray)
            {
                prefabList.Add(AssetDatabase.LoadAssetAtPath<GameObject>(path));
                prefabLength++;
            }

            if (prefabList.Count > 0)
            {
                prefabLists.Add(prefabList);
                prefabTypeList.Add(prefabList[0].GetComponent<Enemy>().EnemySpawnDataType);
            }
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
        if (enemySpawnDataList == null || index < 0 || enemySpawnDataList.Count <= index)
        {
            selectedEnemySpawnData = null;
            selectedEnemySpawnDataIndex = -1;

            selectedPrefab = null;
            selectedEnemyEditorGUI = null;
        }
        else
        {
            selectedEnemySpawnData = enemySpawnDataList[index];
            selectedEnemySpawnDataIndex = index;

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
        string enemySpawnDataFolderPath = $"{GetStageDirectoryPath(selectedStage)}/EnemySpawnData";
        string[] enemySpawnDataPathArray = Directory.GetFiles(enemySpawnDataFolderPath, "*.asset");
        
        enemySpawnDataList.Clear();
        foreach (string enemySpawnDataPath in enemySpawnDataPathArray)
        {
            enemySpawnDataList.Add(AssetDatabase.LoadAssetAtPath<EnemySpawnData>(enemySpawnDataPath));
        }
        enemySpawnDataList.Sort((a, b) => a.spawnTime.CompareTo(b.spawnTime));
    }

    #endregion
}