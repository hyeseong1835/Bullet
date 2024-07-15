using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "Data", menuName = "StageEditor/Data")]
public class StageEditorData : ScriptableObject
{
    public Dictionary<float, bool> timeFoldout { get; private set; } = new Dictionary<float, bool>();

    //Stage
    public Stage selectedStage { get; private set; }
    public Stage[] stageArray { get; private set; }
    public int selectedStageIndex { get; private set; }

    //EnemySpawnData
    public EditorEnemyData selectedEnemyData { get; private set; }
    public int selectedEnemyDataIndex { get; private set; } = -1;
    
    public List<EditorEnemyData> sameTimeEnemyList { get; private set; } = new List<EditorEnemyData>();

    public List<EditorEnemyData> editorEnemySpawnDataList { get; private set; } = new List<EditorEnemyData>();

    //Prefab
    public List<List<GameObject>> prefabLists { get; private set; } = new List<List<GameObject>>();
    public List<Type> prefabTypeList { get; private set; } = new List<Type>();
    
    public int prefabLength { get; private set; }

    public Vector2 previewPos;
    
    public float cellSize = 50;

    public float inspectorLinePosX = 0;
    public float fileViewerLinePosX = 0;

    #region Event

    private void OnEnable()
    {

    }
    private void OnDisable()
    {
        
    }

    #endregion

    #region Stage

    public int SelectStage(Stage stage)
    {
        if (stage == null)
        {
            selectedStage = null;
            return selectedStageIndex = -1;
        }
        for(int index = 0; index < stageArray.Length; index++)
        {
            if (stageArray[index] == stage)
            {
                SelectStage(index);
                return index;
            }
        }
        return -1;
    }
    public Stage SelectStage(int index)
    {
        if (index == -1)
        {
            selectedStage = null;
            selectedStageIndex = -1;
            return null;
        }

        if(index < 0 || stageArray.Length <= index)
        {
            Debug.LogWarning($"Can't Select Stage (Out of Range ({index}/{stageArray.Length - 1}))");
            selectedStage = null;
            selectedStageIndex = -1;
            return null;
        }

        string stagePath = GetStagePath(stageArray[index]);

        selectedStageIndex = index;
        selectedStage = AssetDatabase.LoadAssetAtPath<Stage>(stagePath);
        
        RefreshPrefabList();
        RefreshEnemySpawnDataList();

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
        ApplyPrefabListToStageEnemyPrefabs();
        ApplyEnemySpawnData();
        foreach (EnemySpawnData data in selectedStage.enemySpawnData)
        {
            EditorUtility.SetDirty(data);
        }
        EditorUtility.SetDirty(selectedStage);
     
        EditorUtility.SetDirty(this);

        foreach (EditorEnemyData data in editorEnemySpawnDataList)
        {
            //EditorUtility.SetDirty(data);
        }
    }

    #endregion

    #region Prefab

    public void ApplyPrefabListToStageEnemyPrefabs()
    {
        selectedStage.enemyPrefabArrayCounts = prefabLists.Select((list) => list.Count).ToArray();
        selectedStage.enemyPrefabs = GetAllPrefabArray();
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
    public GameObject[] GetAllPrefabArray()
    {
        return GetAllPrefabList().ToArray();
    }
    public List<GameObject> GetPrefabList(EnemySpawnData enemyData) => GetPrefabList(enemyData.GetType());
    public List<GameObject> GetPrefabList(Type spawnDataType)
    {
        int index = GetPrefabListIndex(spawnDataType);
        return prefabLists[index];
    }
    public int GetPrefabListIndex(EnemySpawnData enemyData) => GetPrefabListIndex(enemyData.GetType());
    public int GetPrefabListIndex(Type spawnDataType)
    {
        int index = prefabTypeList.IndexOf(spawnDataType);
        if (index == -1) Debug.LogError("Can't Find PrefabList");
        
        return index;
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
        foreach (EditorEnemyData enemyData in editorEnemySpawnDataList)
        {
            if (enemyData.spawnData.spawnTime != prevTime)
            {
                if (newTimeFoldout.ContainsKey(enemyData.spawnData.spawnTime) == false)
                {
                    bool foldout;
                    if (!timeFoldout.TryGetValue(enemyData.spawnData.spawnTime, out foldout)) foldout = false;
                    
                    newTimeFoldout.Add(enemyData.spawnData.spawnTime, foldout);
                }
            }
        }
        timeFoldout = newTimeFoldout;
    }

    #endregion
    
    #region EnemySpawnData

    public int SelectEnemyData(EditorEnemyData data)
    {
        selectedEnemyData?.editorGUI.OnDeSelected(selectedEnemyData);

        if (data == null)
        {
            SelectEnemyData(-1);
            return -1;
        }
        int index = editorEnemySpawnDataList.IndexOf(data);
        if (index == -1)
        {
            Debug.LogWarning($"Can't Find EnemySpawnData({data.spawnData.name})");
            SelectEnemyData(-1);
            return -1;
        }
        SelectEnemyData(editorEnemySpawnDataList.IndexOf(data));
        
        return selectedEnemyDataIndex = index;
    }
    public int SelectEnemyData(EnemySpawnData data)
    {
        int dataIndex = editorEnemySpawnDataList.TakeWhile((enemy) => enemy.spawnData != data).Count();
        SelectEnemyData(dataIndex);
        return dataIndex;
    }
    public EditorEnemyData SelectEnemyData(int index)
    {
        if (editorEnemySpawnDataList == null || index < 0 || editorEnemySpawnDataList.Count <= index)
        {
            if (selectedEnemyData != null)
            {
                selectedEnemyData.editorGUI.OnDeSelected(selectedEnemyData);
                selectedEnemyData = null;
                selectedEnemyDataIndex = -1;
            }

            foreach (EditorEnemyData editorEnemyData in sameTimeEnemyList)
            {
                editorEnemyData.editorGUI.OnDeSelected(editorEnemyData);
            }
            sameTimeEnemyList.Clear();

            return null;
        }
        SelectEnemyData(-1);

        float prevSpawnTime = selectedEnemyData?.spawnData.spawnTime ?? -1;

        if (selectedEnemyDataIndex != index)
        {
            selectedEnemyData?.editorGUI.OnDeSelected(selectedEnemyData);
            selectedEnemyData = editorEnemySpawnDataList[index];
            selectedEnemyDataIndex = index;
            selectedEnemyData.editorGUI.OnSelected(selectedEnemyData);
        }

        if (selectedEnemyData.spawnData.spawnTime == prevSpawnTime)
        {
            sameTimeEnemyList.Remove(selectedEnemyData);
            
            selectedEnemyData = editorEnemySpawnDataList[index];
            selectedEnemyDataIndex = index;

            sameTimeEnemyList.Add(selectedEnemyData);
        }
        else
        {
            foreach (EditorEnemyData enemyData in sameTimeEnemyList)
            {
                enemyData.editorGUI.OnDeSelected(enemyData);
            }
            sameTimeEnemyList.Clear();

            for (int i = selectedEnemyDataIndex + 1; i < editorEnemySpawnDataList.Count; i++)
            {
                EditorEnemyData enemyData = editorEnemySpawnDataList[i];

                if (selectedEnemyData.spawnData.spawnTime == enemyData.spawnData.spawnTime)
                {
                    sameTimeEnemyList.Add(enemyData);
                    enemyData.editorGUI.OnSelected(enemyData);
                }
                else break;
            }
            for (int i = selectedEnemyDataIndex - 1; i >= 0; i--)
            {
                EditorEnemyData enemyData = editorEnemySpawnDataList[i];
                if (selectedEnemyData.spawnData.spawnTime == enemyData.spawnData.spawnTime)
                {
                    sameTimeEnemyList.Add(enemyData);
                    enemyData.editorGUI.OnSelected(enemyData);
                }
                else break;
            }
        }
        
        return selectedEnemyData;
    }
    public int InsertToEditorEnemySpawnDataList(EditorEnemyData editorEnemyData)
    {
        for (int index = 0; index < editorEnemySpawnDataList.Count; index++)
        {
            EditorEnemyData enemy = editorEnemySpawnDataList[index];
            if (enemy.spawnData.spawnTime > editorEnemyData.spawnData.spawnTime)
            {
                editorEnemySpawnDataList.Insert(index, editorEnemyData);
                return index;
            }
        }
        editorEnemySpawnDataList.Add(editorEnemyData);
        return editorEnemySpawnDataList.Count - 1;
    }
    public int SortSelectedEnemyData()
    {
        EditorEnemyData data = selectedEnemyData;
        editorEnemySpawnDataList.RemoveAt(selectedEnemyDataIndex);

        return selectedEnemyDataIndex = InsertToEditorEnemySpawnDataList(data);
    }
    
    public void RefreshEnemySpawnDataList()
    {
        if (selectedStage == null)
        {
            Debug.LogWarning("Fail RefreshEnemySpawnDataList (selectedStage is null)");
            editorEnemySpawnDataList.Clear();
            return;
        }
        string enemySpawnDataFolderPath = $"{GetStageDirectoryPath(selectedStage)}/EnemySpawnData";
        string[] enemySpawnDataPathArray = Directory.GetFiles(enemySpawnDataFolderPath, "*.asset");
        
        editorEnemySpawnDataList.Clear();
        foreach (string enemySpawnDataPath in enemySpawnDataPathArray)
        {
            EnemySpawnData spawnData = AssetDatabase.LoadAssetAtPath<EnemySpawnData>(enemySpawnDataPath);
            EditorEnemyData enemyData = new EditorEnemyData(spawnData, typeof(InstantEnemySpawnData));
            editorEnemySpawnDataList.Add(enemyData);
        }
        editorEnemySpawnDataList.Sort((a, b) => a.spawnData.spawnTime.CompareTo(b.spawnData.spawnTime));
    }
    public void ApplyEnemySpawnData()
    {
        //RefreshEnemySpawnDataList();
        
        //selectedStage.enemySpawnData = enemyList.Select((enemy) => enemy.spawnData).ToArray();
    }

    #endregion

    #region Preview

    

    #endregion
}