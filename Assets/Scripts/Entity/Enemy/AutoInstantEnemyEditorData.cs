#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public class AutoInstantEnemyEditorData : EnemyEditorData
{
    public AutoInstantEnemySpawnData spawnData { get; private set; }
    public override EnemySpawnData SpawnData {
        get => spawnData; 
        protected set => spawnData = (AutoInstantEnemySpawnData)value;
    }
    public override Type EnemyType => typeof(AutoInstantEnemy);

    static Event e => UnityEngine.Event.current;

    public AutoInstantEnemyEditorData(AutoInstantEnemySpawnData spawnData)
    {
        this.spawnData = spawnData;
        prefab = SelectPrefab(spawnData.prefabIndex);
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
    public override void DrawInspectorGUI()
    {
        spawnData.startPos = EditorGUILayout.Vector2Field("World Pos", spawnData.startPos);
    }
    public override void Render()
    {
        if (preview == null) return;
        
        preview.transform.position = spawnData.startPos;
        Vector2 startScreenPos = StageEditor.WorldToScreenPos(spawnData.startPos);
        Vector2 dir = (e.mousePosition - startScreenPos);
        preview.transform.rotation = Quaternion.Euler(0, 0, -90 + Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }
    public override void DrawSelectedEnemyDataGizmos()
    {
        Vector2 startScreenPos = StageEditor.WorldToScreenPos(spawnData.startPos);
        //y = (m.x - p.x) / (m.y - p.y) * (x - p.x) + p.y;
        //y = 
        Vector2 endScreenPos = startScreenPos;
        Vector2 dir = (e.mousePosition - startScreenPos);

        Vector2 absDir = dir.Abs();
        if (absDir.x > absDir.y)
        {
            if (absDir.x > 0)
            {
                
            }
        }
        Handles.color = Color.cyan;
        CustomGUI.DrawArrow(startScreenPos, endScreenPos, 0.25f * Mathf.PI, 25);
        Handles.color = Color.white;
    }
    public override void DrawSameTimeEnemyDataGizmos()
    {
        Vector2 startScreenPos = StageEditor.WorldToScreenPos(spawnData.startPos);
        Vector2 endScreenPos = e.mousePosition;

        Handles.color = new Color(0, 0.25f, 0.25f, 1);
        CustomGUI.DrawArrow(startScreenPos, endScreenPos, 0.25f * Mathf.PI, 25);
        Handles.color = Color.white;
    }
}
#endif
