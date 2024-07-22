#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;

public class InstantEnemyEditorData : EnemyEditorData
{
    public override Type EnemyType => typeof(InstantEnemy);
    public InstantEnemySpawnData spawnData { get; private set; }
    public override EnemySpawnData SpawnData
    {
        get => spawnData; 
        protected set => spawnData = (InstantEnemySpawnData)value;
    }
    static Event e => UnityEngine.Event.current;
    public InstantEnemyEditorData(InstantEnemySpawnData spawnData)
    {
        this.spawnData = spawnData;
        prefab = SelectPrefab(spawnData.prefabIndex);
    }
    public override void DrawInspectorGUI()
    {
        spawnData.startPos = EditorGUILayout.Vector2Field("World Pos", spawnData.startPos);
        spawnData.endPos = EditorGUILayout.Vector2Field("Definition", spawnData.endPos);
    }

    public override void DrawSelectedEnemyDataGizmos()
    {
        Vector2 startScreenPos = StageEditor.WorldToScreenPos(spawnData.startPos);
        Vector2 endScreenPos = StageEditor.WorldToScreenPos(spawnData.endPos);

        Handles.color = Color.cyan;
        CustomGUI.DrawArrow(startScreenPos, endScreenPos, 0.25f * Mathf.PI, 25);
        Handles.color = Color.white;
    }
    public override void Render()
    {
        if (preview == null) return;
        
        preview.transform.position = spawnData.startPos;
    }
    public override void DrawSameTimeEnemyDataGizmos()
    {
        Vector2 startScreenPos = StageEditor.WorldToScreenPos(spawnData.startPos);
        Vector2 endScreenPos = StageEditor.WorldToScreenPos(spawnData.endPos);

        Handles.color = new Color(0, 0.25f, 0.25f, 1);
        CustomGUI.DrawArrow(startScreenPos, endScreenPos, 0.25f * Mathf.PI, 25);
        Handles.color = Color.white;
    }
}
#endif


