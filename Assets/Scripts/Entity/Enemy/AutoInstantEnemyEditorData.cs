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
    Vector2 dir;
    Vector2 prevMousePos = Vector2.zero;
    public override void DrawInspectorGUI()
    {
        spawnData.startPos = EditorGUILayout.Vector2Field("World Pos", spawnData.startPos);
    }
    public override void Render()
    {
        if (preview == null) return;
        
        preview.transform.position = spawnData.startPos;
        preview.transform.rotation = Quaternion.Euler(
            0, 
            0, 
            -90 + Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg
        );
    }
    public override void DrawSelectedEnemyDataGizmos()
    {
        Vector2 startScreenPos = StageEditor.WorldToScreenPos(spawnData.startPos);
        dir = e.mousePosition - startScreenPos;

        Handles.color = Color.cyan;
        CustomGUI.DrawArrow(startScreenPos, e.mousePosition, 0.25f * Mathf.PI, 25);
        Handles.color = Color.white;

        if(e.mousePosition != prevMousePos)
        {
            StageEditor.instance.Repaint();
            prevMousePos = e.mousePosition;
        }
    }
    public override void DrawSameTimeEnemyDataGizmos()
    {
        Vector2 startScreenPos = StageEditor.WorldToScreenPos(spawnData.startPos);

        Handles.color = new Color(0, 0.25f, 0.25f, 1);
        CustomGUI.DrawArrow(startScreenPos, e.mousePosition, 0.25f * Mathf.PI, 25);
        Handles.color = Color.white;
        
        if (e.mousePosition != prevMousePos)
        {
            StageEditor.instance.Repaint();
            prevMousePos = e.mousePosition;
        }
    }
}
#endif
