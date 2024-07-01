using UnityEditor;
using UnityEngine;

public class InstantEnemyEditorGUI : EnemyEditorGUI
{
    public override void DrawInspectorGUI(EnemySpawnData spawnData)
    {
        InstantEnemySpawnData instantEnemy = (InstantEnemySpawnData)spawnData;

        instantEnemy.startPos = EditorGUILayout.Vector2Field("World Pos", instantEnemy.startPos);
        instantEnemy.endPos = EditorGUILayout.Vector2Field("Definition", instantEnemy.endPos);
    }

    public override void DrawEnemyDataGizmos(EnemySpawnData spawnData)
    {
        InstantEnemySpawnData instantEnemy = (InstantEnemySpawnData)spawnData;

        Vector2 screenPos = StageEditor.WorldToScreenPos(instantEnemy.startPos);
        Handles.color = Color.cyan;

        Handles.DrawWireDisc(screenPos, Vector3.forward, 10);

        Vector2 definitionScreenPos = StageEditor.WorldToScreenPos(instantEnemy.endPos);

        Handles.DrawLine(screenPos, definitionScreenPos);
        Vector2 X = new Vector2(StageEditor.setting.definitionGizmoSize, -StageEditor.setting.definitionGizmoSize);
        Handles.DrawLine(definitionScreenPos + X, definitionScreenPos - X);
        Handles.DrawLine(definitionScreenPos + Vector2.one * StageEditor.setting.definitionGizmoSize, definitionScreenPos - Vector2.one * StageEditor.setting.definitionGizmoSize);
        Handles.color = Color.white;
    }
}
