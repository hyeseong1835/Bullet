using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

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
    public int selectedStageIndex;
    public string[] stageNameArray;

    public EnemySpawnData selectedEnemySpawnData { get; private set; }
    public EnemyEditorGUI selectedEnemyEditorGUI { get; private set; }
    public int selectedEnemyIndex = -1;

    public List<EnemySpawnData> enemySpawnDataList = new List<EnemySpawnData>();

    public GameObject[] prefabs = new GameObject[0];

    public Box preview;
    public Vector2 previewPos;

    public float cellSize = 50;

    public float inspectorLinePosX;
    public float filesLinePosX;
    public float enemyScroll = 0;
    public float timeLength = 60;
    public Dictionary<float, bool> timeFoldout = new Dictionary<float, bool>();
    private void OnValidate()
    {
        preview = new Box();
        preview.size = new Vector2(Window.GameWidth, Window.GameHeight) * cellSize;
        preview.center = new Vector2(0, -0.5f * Window.GameHeight * cellSize);
    }
    public EnemySpawnData SelectEnemySpawnData(int index)
    {
        selectedEnemyIndex = index;

        if (index == -1)
        {
            selectedEnemySpawnData = null;
            selectedEnemyEditorGUI = null;
        }
        else
        {
            selectedEnemySpawnData = enemySpawnDataList[index];
            selectedEnemyEditorGUI = StageEditor.GetEnemyEditor(selectedEnemySpawnData.EditorType);
        }
        return selectedEnemySpawnData;
    }
    public int SelectEnemySpawnData(EnemySpawnData spawnData)
    {
        selectedEnemySpawnData = spawnData;

        if (spawnData == null)
        {
            selectedEnemyIndex = -1;
            selectedEnemyEditorGUI = null;
        }
        else
        {
            selectedEnemyIndex = enemySpawnDataList.IndexOf(spawnData);
            selectedEnemyEditorGUI = StageEditor.GetEnemyEditor(spawnData.EditorType);
        }
        return selectedEnemyIndex;
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
    public void RefreshEnemyDataList()
    {
        string stageDataPath = AssetDatabase.GetAssetPath(selectedStage);
        string stageDataResourcePath = stageDataPath.Substring("Assets/Resources/".Length);
        string stageFolderResourcePath = stageDataResourcePath.Substring(0, stageDataResourcePath.Length - (selectedStage.name.Length + ".assets".Length));
        
        enemySpawnDataList = Resources.LoadAll<EnemySpawnData>(stageFolderResourcePath).ToList();
        enemySpawnDataList.Sort((a, b) => a.spawnTime.CompareTo(b.spawnTime));
    }
}
