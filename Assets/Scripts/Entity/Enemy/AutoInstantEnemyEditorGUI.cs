#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class AutoInstantEnemyEditorGUI : EnemyEditorGUI
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
        AutoInstantEnemySpawnData data = (AutoInstantEnemySpawnData)enemyData.spawnData;

        data.startPos = EditorGUILayout.Vector2Field("World Pos", data.startPos);
    }
    public override void Render(EditorEnemyData enemyData)
    {
        if (enemyData.preview == null) return;

        AutoInstantEnemySpawnData data = (AutoInstantEnemySpawnData)enemyData.spawnData;
        
        enemyData.preview.transform.position = data.startPos;
    }
}
#endif
