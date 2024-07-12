using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class InstantEnemyEditorGUI : EnemyEditorGUI
{
    static Event e => UnityEngine.Event.current;
    Dictionary<EditorEnemyData, GameObject> instance = new Dictionary<EditorEnemyData, GameObject>();

    public override void OnSelected(EditorEnemyData enemyData)
    {
        GameObject obj = StageEditor.instance.previewRender.InstantiatePrefabInScene(enemyData.prefab);
        Debug.Log($"Selected: {obj.name}#{obj.GetHashCode()}");
        instance.Add(enemyData, obj);
        obj.transform.position = ((InstantEnemySpawnData)(enemyData.spawnData)).startPos;
    }
    public override void OnDeSelected(EditorEnemyData enemyData)
    {
        Debug.Log($"DeSelected: {instance[enemyData].name}#{instance[enemyData].GetHashCode()}");
        Object.DestroyImmediate(instance[enemyData]);
        instance.Remove(enemyData);
    }

    public override void Event()
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                switch (e.button)
                {
                    case 0: Mouse0Down(); break;
                }
                break;

            case EventType.MouseDrag:
                switch (e.button)
                {
                    case 0: Mouse0Drag(); break;
                }
                break;

            case EventType.MouseUp:
                switch (e.button)
                {
                    case 0: Mouse0Up(); break;
                }
                break;
        }
        void Mouse0Down()
        {

        }
        void Mouse0Drag()
        {

        }
        void Mouse0Up()
        {

        }
    }
    public override void DrawInspectorGUI(EditorEnemyData enemyData)
    {
        InstantEnemySpawnData data = (InstantEnemySpawnData)enemyData.spawnData;

        data.startPos = EditorGUILayout.Vector2Field("World Pos", data.startPos);
        data.endPos = EditorGUILayout.Vector2Field("Definition", data.endPos);
    }

    public override void DrawSelectedEnemyDataGizmos(EditorEnemyData enemyData)
    {
        InstantEnemySpawnData data = (InstantEnemySpawnData)enemyData.spawnData;

        Vector2 startScreenPos = StageEditor.WorldToScreenPos(data.startPos);
        Vector2 endScreenPos = StageEditor.WorldToScreenPos(data.endPos);

        Handles.color = Color.cyan;
        CustomGUI.DrawArrow(startScreenPos, endScreenPos, 0.25f * Mathf.PI, 25);
        Handles.color = Color.white;
    }
    public override void Render(PreviewRenderUtility renderer, EditorEnemyData enemyData)
    {
        if (enemyData.prefab == null)
        {
            Debug.LogError("Prefab is null");
            return;
        }

        InstantEnemySpawnData data = (InstantEnemySpawnData)enemyData.spawnData;

        GameObject obj = renderer.InstantiatePrefabInScene(enemyData.prefab);
        obj.transform.position = Vector3.zero;
        renderer.camera.transform.LookAt(obj.transform);
        
        Debug.Log($"Render: {enemyData.prefab}");
    }
    public override void DrawSameTimeEnemyDataGizmos(EditorEnemyData enemyData)
    {
        InstantEnemySpawnData data = (InstantEnemySpawnData)enemyData.spawnData;

        Vector2 startScreenPos = StageEditor.WorldToScreenPos(data.startPos);
        Vector2 endScreenPos = StageEditor.WorldToScreenPos(data.endPos);

        Handles.color = new Color(0, 0.25f, 0.25f, 1);
        CustomGUI.DrawArrow(startScreenPos, endScreenPos, 0.25f * Mathf.PI, 25);
        Handles.color = Color.white;
    }
}
