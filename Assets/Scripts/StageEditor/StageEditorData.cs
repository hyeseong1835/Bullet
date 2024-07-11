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
    static StageEditor Editor => StageEditor.instance;

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
    
    public PreviewRenderUtility previewRender;

    public float cellSize = 50;

    public float inspectorLinePosX;
    public float fileViewerLinePosX;

    public float enemyScroll;
    public float timeLength;

    public float timeMoveSnap = 0.5f;

    public List<EditorEnemyData> drawEditorEnemyDataList = new List<EditorEnemyData>();

    #region Event

    private void OnEnable()
    {
        PreviewInit();
    }
    private void OnDisable()
    {
        foreach (EditorEnemyData enemyData in drawEditorEnemyDataList)
        {
            ExcludeDrawEnemy(enemyData);
        }
        previewRender.Cleanup();
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

        foreach (EditorEnemyData data in enemyList)
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

    #region DrawPreview

    public RenderTexture CreatePreviewTexture()
    {
        previewRender.camera.transform.position = ((Vector3)StageEditor.ScreenToWorldPoint(Editor.previewRect.GetCenter())).GetSetZ(-10);
        previewRender.camera.orthographicSize = 0.5f * (Editor.previewRect.height / cellSize);

        previewRender.BeginPreview(Editor.previewRect, GUIStyle.none);

        //previewRender.lights[0].transform.localEulerAngles = new Vector3(30, 30, 0);
        previewRender.lights[0].intensity = 2;

        previewRender.camera.Render();

        return (RenderTexture)previewRender.EndPreview();
    }
    public void PreviewInit()
    {
        foreach (EditorEnemyData enemyData in drawEditorEnemyDataList)
        {
            ExcludeDrawEnemy(enemyData);
        }

        if (previewRender != null)
            previewRender.Cleanup();

        previewRender = new PreviewRenderUtility(true);

        System.GC.SuppressFinalize(previewRender);

        var camera = previewRender.camera;
        Camera gameCam = CameraController.instance.cam;
        camera.orthographic = gameCam.orthographic;
        camera.orthographicSize = 1;

        camera.transform.rotation = gameCam.transform.rotation;

        camera.clearFlags = gameCam.clearFlags;
        camera.backgroundColor = StageEditor.setting.previewBackGroundColor;
        camera.cullingMask = gameCam.cullingMask;

        camera.fieldOfView = gameCam.fieldOfView;

        camera.nearClipPlane = gameCam.nearClipPlane;

        camera.farClipPlane = gameCam.farClipPlane;
    }
    public void DrawEnemyPreview(EditorEnemyData enemyData)
    {
        if (drawEditorEnemyDataList.Contains(enemyData) == false)
        {
            enemyData.editorGUI.OnSelected(enemyData);
            drawEditorEnemyDataList.Add(enemyData);
        }
    }
    public void ExcludeDrawEnemy(EditorEnemyData enemyData)
    {
        enemyData.editorGUI.OnDeSelected(enemyData);
        drawEditorEnemyDataList.Remove(enemyData);
    }

    #endregion

    #region EnemySpawnData

    public int SelectEnemySpawnData(EditorEnemyData data)
    {
        if (data == null)
        {
            SelectEnemyData(-1);
            return -1;
        }
        int index = enemyList.IndexOf(data);
        if (index == -1)
        {
            Debug.LogWarning($"Can't Find EnemySpawnData({data.spawnData.name})");
            SelectEnemyData(-1);
            return -1;
        }
        selectedEnemyData = data;
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
            selectedEnemyData = null;
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
                return selectedEnemyDataIndex = index;
            }
        }
        enemyList.Add(data);
        return selectedEnemyDataIndex = enemyList.Count - 1;
    }
    
    public void RefreshEnemySpawnDataList()
    {
        if (selectedStage == null)
        {
            Debug.LogWarning("Fail RefreshEnemySpawnDataList (selectedStage is null)");
            enemyList.Clear();
            return;
        }
        string enemySpawnDataFolderPath = $"{GetStageDirectoryPath(selectedStage)}/EnemySpawnData";
        string[] enemySpawnDataPathArray = Directory.GetFiles(enemySpawnDataFolderPath, "*.asset");
        
        enemyList.Clear();
        foreach (string enemySpawnDataPath in enemySpawnDataPathArray)
        {
            EnemySpawnData spawnData = AssetDatabase.LoadAssetAtPath<EnemySpawnData>(enemySpawnDataPath);
            EditorEnemyData enemyData = new EditorEnemyData(spawnData, typeof(InstantEnemySpawnData));
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