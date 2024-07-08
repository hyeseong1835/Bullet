using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EditorEnemyData
{
    public EnemySpawnData spawnData;
    public EnemyEditorGUI editorGUI => unSafeEditorGUI ?? SetEditorGUI();
    public EnemyEditorGUI unSafeEditorGUI { get; private set; }

    public GameObject prefab;
    public Type prefabType;

    public EditorEnemyData(Stage stage, EnemySpawnData spawnData)
    {
        this.spawnData = spawnData;

        prefab = SelectPrefab(spawnData.prefabIndex);
        prefabType = prefab.GetComponent<Enemy>().GetType();
    }
    public void Apply()
    {
        StageEditor.data.RefreshPrefabList();
        ApplyPrefab();
     
        EditorUtility.SetDirty(StageEditor.data);
    }

    #region EditorGUI

    public EnemyEditorGUI SetEditorGUI()
    {
        unSafeEditorGUI = StageEditor.data.GetEnemyEditor(spawnData.EditorType);

        return unSafeEditorGUI;
    }

    #endregion

    #region Prefab

    public void ApplyPrefab()
    {
        spawnData.prefabIndex = StageEditor.data.GetPrefabList(prefabType).IndexOf(prefab);
    }
    public void SetPrefab(GameObject prefab)
    {
        this.prefab = prefab;
        prefabType = prefab.GetComponent<Enemy>().GetType();
    }
    public GameObject SelectPrefab(int index)
    {
        List<GameObject> prefabList = StageEditor.data.GetPrefabList(prefabType);
        if (prefabList.Count <= index)
        {
            if (prefabList.Count > 0)
            {
                index = 0;
                Debug.LogWarning("Can't Select Prefab (Out of Index)");
            }
            else
            {
                prefab = null;
                spawnData.prefabIndex = -1;
                Debug.LogError("Can't Select Prefab (PrefabList is Empty)");
                return null;
            }
        }
        spawnData.prefabIndex = index;
        return prefab = prefabList[index];
    }

    #endregion
}
[CreateAssetMenu(fileName = "Data", menuName = "StageEditor/Data")]
public class StageEditorData : ScriptableObject
{
    public static Dictionary<Type, EnemyEditorGUI> enemyEditorGUIDictionary = new Dictionary<Type, EnemyEditorGUI>();
    public Dictionary<float, bool> timeFoldout { get; private set; } = new Dictionary<float, bool>();

    //Stage
    public Stage selectedStage { get; private set; }
    public Stage[] stageArray { get; private set; }
    public int selectedStageIndex { get; private set; }

    //EnemySpawnData
    public EditorEnemyData selectedEnemyData { get; private set; }
    public int selectedEnemyDataIndex { get; private set; } = -1;
    
    public List<EditorEnemyData> enemyList { get; private set; } = new List<EditorEnemyData>();

    //Prefab
    public List<List<GameObject>> prefabLists { get; private set; } = new List<List<GameObject>>();
    public List<Type> prefabTypeList { get; private set; } = new List<Type>();
    
    public int prefabLength { get; private set; }

    public Vector2 previewPos;

    public float cellSize = 50;

    public float inspectorLinePosX;
    public float fileViewerLinePosX;

    public float enemyScroll;
    public float timeLength;

    public float timeMoveSnap = 0.5f;

    #region Stage
    public int SelectStage(Stage stage)
    {
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

        foreach (EditorEnemyData data in enemyList)
        {
            //EditorUtility.SetDirty(data);
        }
    }//

    #endregion

    #region Prefab

    public void ApplyPrefabListToStageEnemyPrefabs()
    {
        selectedStage.enemyPrefabs = prefabLists.Select((list) => list.ToArray()).ToArray();
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
        foreach (EditorEnemyData enemyData in enemyList)
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

    public int SelectEnemySpawnData(EditorEnemyData data)
    {
        int index = enemyList.IndexOf(data);
        selectedEnemyData = enemyList[index];
        return selectedEnemyDataIndex = index;
    }
    public int SelectEnemySpawnData(EnemySpawnData data)
    {
        int dataIndex = enemyList.TakeWhile((enemy) => enemy.spawnData != data).Count();
        SelectEnemyData(dataIndex);
        return dataIndex;
    }
    public EditorEnemyData SelectEnemyData(int index)
    {
        if (enemyList == null || index < 0 || enemyList.Count <= index)
        {
            selectedEnemyData = default;
            selectedEnemyDataIndex = -1;
        }
        else
        {
            selectedEnemyData = enemyList[index];
            selectedEnemyDataIndex = index;
        }
        return selectedEnemyData;
    }
    public int SortSelectedEnemyData()
    {
        EditorEnemyData data = selectedEnemyData;
        enemyList.Remove(selectedEnemyData);

        for (int index = 0; index < enemyList.Count; index++)
        {
            EditorEnemyData enemy = enemyList[index];
            if (enemy.spawnData.spawnTime > data.spawnData.spawnTime)
            {
                enemyList.Insert(index, data);
                selectedEnemyDataIndex = index;
                return index;
            }
        }
        enemyList.Add(data);
        selectedEnemyDataIndex = enemyList.Count - 1;
        return selectedEnemyDataIndex;
    }
    
    public void RefreshEnemySpawnDataList()
    {
        if (selectedStage == null)
        {
            enemyList.Clear();
            return;
        }
        string enemySpawnDataFolderPath = $"{GetStageDirectoryPath(selectedStage)}/EnemySpawnData";
        string[] enemySpawnDataPathArray = Directory.GetFiles(enemySpawnDataFolderPath, "*.asset");
        
        enemyList.Clear();
        foreach (string enemySpawnDataPath in enemySpawnDataPathArray)
        {
            EnemySpawnData spawnData = AssetDatabase.LoadAssetAtPath<EnemySpawnData>(enemySpawnDataPath);
            EditorEnemyData enemyData = new EditorEnemyData(selectedStage, spawnData);
            enemyList.Add(enemyData);
        }
        enemyList.Sort((a, b) => a.spawnData.spawnTime.CompareTo(b.spawnData.spawnTime));
    }
    public void ApplyEnemySpawnData()
    {
        //RefreshEnemySpawnDataList();
        
        //selectedStage.enemySpawnData = enemyList.Select((enemy) => enemy.spawnData).ToArray();
    }
    public EnemyEditorGUI GetEnemyEditor(EnemySpawnData spawnData) => GetEnemyEditor(spawnData.EditorType);
    public EnemyEditorGUI GetEnemyEditor(Type editorType)
    {
        EnemyEditorGUI editorInstance;

        if (enemyEditorGUIDictionary.TryGetValue(editorType, out editorInstance) == false)
        {
            editorInstance = (EnemyEditorGUI)Activator.CreateInstance(editorType);
            enemyEditorGUIDictionary.Add(editorType, editorInstance);
        }
        return editorInstance;
    }

    #endregion
}