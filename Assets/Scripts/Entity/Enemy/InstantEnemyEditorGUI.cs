#if UNITY_EDITOR
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
        CustomGUI.DrawArrow(startScreenPos, endScreenPos, 0.25f * Mathf.PI, 25);
        Handles.color = Color.white;
    }
    public override void Render(EditorEnemyData enemyData)
    {
        if (enemyData.preview == null) return;
        
        InstantEnemySpawnData data = (InstantEnemySpawnData)enemyData.spawnData;

        enemyData.preview.transform.position = data.startPos;
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
#endif
