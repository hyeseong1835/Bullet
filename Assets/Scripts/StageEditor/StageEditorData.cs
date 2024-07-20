#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "StageEditor/Data")]
public class StageEditorData : ScriptableObject
{
    public Dictionary<float, bool> timeFoldout { get; private set; } = new Dictionary<float, bool>();

    //Stage
    public Stage selectedStage { get; private set; }
    public Stage[] stageArray { get; private set; }
    public int selectedStageIndex { get; private set; }

    //EnemySpawnData
    public EnemyEditorData selectedEnemyData { get; private set; }
    public int selectedEnemyDataIndex { get; private set; } = -1;
    
    public List<EnemyEditorData> sameTimeEnemyList { get; private set; } = new List<EnemyEditorData>();

    public List<EnemyEditorData> editorEnemySpawnDataList { get; private set; } = new List<EnemyEditorData>();

    //Prefab
    public GameObject[][] prefabDoubleArray { get; private set; }
    public string[] enemyTypeNameArray { get; private set; }
    public string selectedEnemyTypeName { get; private set; }
    public int selectedEnemyTypeNameIndex { get; private set; }
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
        if (selectedStage != null)
        {
            ApplyStage();
        }
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
        
        RefreshPrefab();
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
    public void ApplyStage()
    {
        RefreshPrefab();
        RefreshEnemySpawnDataList();

        ApplyPrefab();
        ApplyEnemySpawnDataArray();
        EditorUtility.SetDirty(selectedStage);
    }

    #endregion

    #region Prefab

    public void ApplyPrefab()
    {
        selectedStage.enemyPrefabFolderNameArray = enemyTypeNameArray;
    }
    public void RefreshPrefab()
    {
        prefabLength = 0;

        if (selectedStage == null) return;

        string enemyPrefabsFolderPath = GetStageDirectoryPath(selectedStage) + "/EnemyPrefabs";
        string[] prefabFolderPath = Directory.GetDirectories(enemyPrefabsFolderPath);

        //EnemyTypeNameArray
        enemyTypeNameArray = new string[prefabFolderPath.Length];
        for (int i = 0; i < enemyTypeNameArray.Length; i++)
        {
            string enemyTypeName = prefabFolderPath[i].Substring(prefabFolderPath[i].LastIndexOf('\\') + 1);
            
            if (selectedEnemyTypeName == enemyTypeName)
            {
                selectedEnemyTypeNameIndex = i;
            }
            enemyTypeNameArray[i] = enemyTypeName;
        }

        //PrefabDoubleArray
        List<List<GameObject>> result = new List<List<GameObject>>();

        for (int folderIndex = 0; folderIndex < prefabFolderPath.Length; folderIndex++)
        {
            List<GameObject> prefabList = new List<GameObject>();

            string[] prefabPathArray = Directory.GetFiles(prefabFolderPath[folderIndex], "*.prefab");
            foreach (string path in prefabPathArray)
            {
                prefabList.Add(AssetDatabase.LoadAssetAtPath<GameObject>(path));
                prefabLength++;
            }

            if (prefabList.Count > 0)
            {
                result.Add(prefabList);
            }
        }
        prefabDoubleArray = result.ToDoubleArray();
    }
    
    #endregion

    #region TimeFoldout
    
    public void RefreshTimeFoldout()
    {
        float prevTime = -1;

        Dictionary<float, bool> newTimeFoldout = new Dictionary<float, bool>();
        foreach (EnemyEditorData enemyData in editorEnemySpawnDataList)
        {
            if (enemyData.SpawnData.spawnTime != prevTime)
            {
                if (newTimeFoldout.ContainsKey(enemyData.SpawnData.spawnTime) == false)
                {
                    bool foldout;
                    if (!timeFoldout.TryGetValue(enemyData.SpawnData.spawnTime, out foldout)) foldout = false;
                    
                    newTimeFoldout.Add(enemyData.SpawnData.spawnTime, foldout);
                }
            }
        }
        timeFoldout = newTimeFoldout;
    }

    #endregion
    
    #region EnemySpawnData

    public Type GetEnemySpawnDataType(string enemyTypeName) => Type.GetType($"{enemyTypeName}SpawnData");

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spawnData"></param>
    /// <returns>생성한 EnemySpawnData의 Asset Path</returns>
    public string CreateEnemySpawnData(EnemySpawnData spawnData)
    {
        string stageDirectoryPath = GetStageDirectoryPath(selectedStage);
        
        bool isExist;
        string path;
        do
        {
            int random = UnityEngine.Random.Range(10000000, 99999999);
            path = $"{stageDirectoryPath}/EnemySpawnData/{random}.asset";
            isExist = Directory.Exists(path);
        }
        while (isExist);

        AssetDatabase.CreateAsset(spawnData, path);
        return path;
    }
    public int SelectEnemyData(EnemySpawnData data)
    {
        int dataIndex = editorEnemySpawnDataList.TakeWhile((enemy) => enemy.SpawnData != data).Count();
        SelectEnemyData(dataIndex);
        return dataIndex;
    }
    public EnemyEditorData SelectEnemyData(int index)
    {
        if (editorEnemySpawnDataList == null || index < 0 || editorEnemySpawnDataList.Count <= index)
        {
            if (selectedEnemyData != null)
            {
                selectedEnemyData.OnDeSelected();
                selectedEnemyData = null;
                selectedEnemyDataIndex = -1;
            }

            foreach (EnemyEditorData editorEnemyData in sameTimeEnemyList)
            {
                editorEnemyData.OnDeSelected();
            }
            sameTimeEnemyList.Clear();

            selectedEnemyTypeName = "Null";
            selectedEnemyTypeNameIndex = -1;
            return null;
        }

        selectedEnemyData?.OnDeSelected();
        selectedEnemyData = editorEnemySpawnDataList[index];
        selectedEnemyDataIndex = index;
        selectedEnemyData.OnSelected();

        //SameTimeEnemyList 초기화 (selectedEnemyData 제외)
        foreach (EnemyEditorData enemyData in sameTimeEnemyList)
        {
            if (enemyData != selectedEnemyData)
            {
                enemyData.OnDeSelected();
            }
        }
        sameTimeEnemyList.Clear();

        //SameTimeEnemyList 추가 (+)
        for (int i = selectedEnemyDataIndex + 1; i < editorEnemySpawnDataList.Count; i++)
        {
            EnemyEditorData enemyData = editorEnemySpawnDataList[i];

            if (selectedEnemyData.SpawnData.spawnTime == enemyData.SpawnData.spawnTime)
            {
                sameTimeEnemyList.Add(enemyData);
                enemyData.OnSelected();
            }
            else break;
        }
        //SameTimeEnemyList 추가 (-)
        for (int i = selectedEnemyDataIndex - 1; i >= 0; i--)
        {
            EnemyEditorData enemyData = editorEnemySpawnDataList[i];
            if (selectedEnemyData.SpawnData.spawnTime == enemyData.SpawnData.spawnTime)
            {
                sameTimeEnemyList.Add(enemyData);
                enemyData.OnSelected();
            }
            else break;
        }

        selectedEnemyTypeName = selectedEnemyData.EnemyType.Name;
        selectedEnemyTypeNameIndex = enemyTypeNameArray.IndexOf(selectedEnemyData.EnemyType.Name);
        return selectedEnemyData;
    }
    public int InsertToEditorEnemySpawnDataList(EnemyEditorData editorEnemyData)
    {
        for (int index = 0; index < editorEnemySpawnDataList.Count; index++)
        {
            EnemyEditorData enemy = editorEnemySpawnDataList[index];
            if (enemy.SpawnData.spawnTime > editorEnemyData.SpawnData.spawnTime)
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
        EnemyEditorData data = selectedEnemyData;
        editorEnemySpawnDataList.RemoveAt(selectedEnemyDataIndex);

        return selectedEnemyDataIndex = InsertToEditorEnemySpawnDataList(data);
    }
    
    public void RefreshEnemySpawnDataList()
    {
        editorEnemySpawnDataList.Clear();
        
        if (selectedStage == null)
        {
            Debug.LogWarning("Fail RefreshEnemySpawnDataList (selectedStage is null)");
            return;
        }
        if (enemyTypeNameArray == null)
        {
            Debug.LogWarning("Fail RefreshEnemySpawnDataList (enemyTypeNameArray is null)");
            return;
        }
        if (enemyTypeNameArray.Length == 0)
        {
            Debug.LogWarning("Fail RefreshEnemySpawnDataList (enemyTypeNameArray is Empty)");
            return;
        }

        for (int folderIndex = 0; folderIndex < enemyTypeNameArray.Length; folderIndex++)
        {
            string folderName = enemyTypeNameArray[folderIndex];
            string enemySpawnDataFolderResourcePath = $"{selectedStage.StageDirectoryResourcePath}/EnemySpawnData";
       
            EnemySpawnData[] enemySpawnDataArray = Resources.LoadAll<EnemySpawnData>($"{enemySpawnDataFolderResourcePath}/{folderName}");
            foreach (EnemySpawnData spawnData in enemySpawnDataArray)
            {
                editorEnemySpawnDataList.Add(spawnData.CreateEditorData());
            }
        }
        editorEnemySpawnDataList.Sort(
            (a, b) => 
                a.SpawnData.spawnTime.CompareTo(b.SpawnData.spawnTime)
        );
    }
    public void ApplyEnemySpawnDataArray()
    {
        for (int i = 0; i < editorEnemySpawnDataList.Count; i++)
        {
            EnemyEditorData editorEnemyData = editorEnemySpawnDataList[i];
            editorEnemyData.Apply();
        }
    }

    #endregion
}
#endif