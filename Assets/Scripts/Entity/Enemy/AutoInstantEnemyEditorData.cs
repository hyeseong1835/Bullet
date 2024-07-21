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
    Vector2 endScreenPos;
    public override void DrawInspectorGUI()
    {
        spawnData.startPos = EditorGUILayout.Vector2Field("World Pos", spawnData.startPos);
    }
    public override void Render()
    {
        if (preview == null) return;
        
        preview.transform.position = spawnData.startPos;
        preview.transform.rotation = Quaternion.Euler(0, 0, -90 + Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }
    public override void DrawSelectedEnemyDataGizmos()
    {
        dir = (StageEditor.ScreenToWorldPoint(e.mousePosition) - spawnData.startPos);

        if (dir.magnitude > 0.1f)
        {
            Vector2 worldPos;
            if (dir.x > 0) worldPos.x = Window.instance.gameRight + 1;
            else worldPos.x = Window.instance.gameLeft - 1;

            if (dir.y * (worldPos.x - spawnData.startPos.x) > dir.x * ((Window.instance.gameTop + 1) - spawnData.startPos.y))
            {
                worldPos.y = Window.instance.gameTop + 1;
                worldPos.x = (dir.x / dir.y) * (worldPos.y - spawnData.startPos.y) + spawnData.startPos.x;
            }
            else if (dir.y * (worldPos.x - spawnData.startPos.x) < dir.x * ((Window.instance.gameBottom - 1) - spawnData.startPos.y))
            {
                worldPos.y = Window.instance.gameBottom - 1;
                worldPos.x = (dir.x / dir.y) * (worldPos.y - spawnData.startPos.y) + spawnData.startPos.x;
            }
            else worldPos.y = (dir.y / dir.x) * (worldPos.x - spawnData.startPos.x) + spawnData.startPos.y;

            endScreenPos = StageEditor.WorldToScreenPos(worldPos);
        }

        Vector2 startScreenPos = StageEditor.WorldToScreenPos(spawnData.startPos);

        Handles.color = Color.cyan;
        CustomGUI.DrawArrow(startScreenPos, endScreenPos, 0.25f * Mathf.PI, 25);
        Handles.color = Color.white;

        if (e.delta != Vector2.zero) StageEditor.instance.Repaint();
    }
    public override void DrawSameTimeEnemyDataGizmos()
    {
        dir = (StageEditor.ScreenToWorldPoint(e.mousePosition) - spawnData.startPos);

        if (dir.magnitude > 0.1f)
        {
            Vector2 worldPos;
            if (dir.x > 0) worldPos.x = Window.instance.gameRight + 1;
            else worldPos.x = Window.instance.gameLeft - 1;

            if (dir.y * (worldPos.x - spawnData.startPos.x) > dir.x * ((Window.instance.gameTop + 1) - spawnData.startPos.y))
            {
                worldPos.y = Window.instance.gameTop + 1; 
                worldPos.x = (dir.x / dir.y) * (worldPos.y - spawnData.startPos.y) + spawnData.startPos.x;
            }
            else if (dir.y * (worldPos.x - spawnData.startPos.x) < dir.x * ((Window.instance.gameBottom - 1) - spawnData.startPos.y))
            {
                worldPos.y = Window.instance.gameBottom - 1;
                worldPos.x = (dir.x / dir.y) * (worldPos.y - spawnData.startPos.y) + spawnData.startPos.x;
            }
            else worldPos.y = (dir.y / dir.x) * (worldPos.x - spawnData.startPos.x) + spawnData.startPos.y;

            endScreenPos = StageEditor.WorldToScreenPos(worldPos);
        }

        Vector2 startScreenPos = StageEditor.WorldToScreenPos(spawnData.startPos);
        
        Handles.color = new Color(0, 0.25f, 0.25f, 1);
        CustomGUI.DrawArrow(startScreenPos, endScreenPos, 0.25f * Mathf.PI, 25);
        Handles.color = Color.white;
        
        if (e.delta != Vector2.zero) StageEditor.instance.Repaint();
    }
}
#endif
