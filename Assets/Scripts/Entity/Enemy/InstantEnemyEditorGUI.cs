using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class InstantEnemyEditorGUI : EnemyEditorGUI
{
    static Event e => UnityEngine.Event.current;
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
        Handles.DrawLine(startScreenPos, endScreenPos);
        Vector2 X = new Vector2(StageEditor.setting.definitionGizmoSize, -StageEditor.setting.definitionGizmoSize);
        Handles.DrawLine(endScreenPos + X, endScreenPos - X);
        Handles.DrawLine(endScreenPos + Vector2.one * StageEditor.setting.definitionGizmoSize, endScreenPos - Vector2.one * StageEditor.setting.definitionGizmoSize);
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
        DrawSelectedEnemyDataGizmos(enemyData);
    }
}
