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
    
    public override void DrawInspectorGUI(EnemySpawnData enemySpawnData)
    {
        InstantEnemySpawnData data = (InstantEnemySpawnData)enemySpawnData;

        data.startPos = EditorGUILayout.Vector2Field("World Pos", data.startPos);
        data.endPos = EditorGUILayout.Vector2Field("Definition", data.endPos);
    }

    public override void DrawEnemyDataGizmos(EnemySpawnData enemySpawnData)
    {
        InstantEnemySpawnData data = (InstantEnemySpawnData)enemySpawnData;

        Vector2 startScreenPos = StageEditor.WorldToScreenPos(data.startPos);
        Vector2 endScreenPos = StageEditor.WorldToScreenPos(data.endPos);

        Handles.color = Color.cyan;
        Handles.DrawWireDisc(startScreenPos, Vector3.forward, 10);

        Handles.DrawLine(startScreenPos, endScreenPos);
        Vector2 X = new Vector2(StageEditor.setting.definitionGizmoSize, -StageEditor.setting.definitionGizmoSize);
        Handles.DrawLine(endScreenPos + X, endScreenPos - X);
        Handles.DrawLine(endScreenPos + Vector2.one * StageEditor.setting.definitionGizmoSize, endScreenPos - Vector2.one * StageEditor.setting.definitionGizmoSize);
        Handles.color = Color.white;
    }
}
