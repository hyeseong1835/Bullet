using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InstantEnemy : Enemy
{
    public InstantEnemyData data;
    public override EnemyData EnemyData => data;

    [SerializeField] Weapon weapon;
    public PushEnemyState pushEnemyState = PushEnemyState.None;

    Vector2 startPos, endPos;
    float speed;


    Vector3 dir;
    void Start()
    {
        dir = endPos - startPos;
        pushEnemyState = PushEnemyState.Push;
    }
    new void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
        if (dir.x != 0) CheckOver(dir.x > 0, transform.position.x, endPos.x);
        else CheckOver(dir.y > 0, transform.position.y, endPos.y);

        void CheckOver(bool isDirPositive, float cur, float end)
        {
            if (isDirPositive ? cur > end : cur < end) Destroy(gameObject);
        }
    }
    public override void DrawStageEditorGUI(Rect rect)
    {
        GUILayout.BeginArea(rect);
        
        data = (InstantEnemyData)EditorGUILayout.ObjectField(data, typeof(InstantEnemyData), false);

        startPos = EditorGUILayout.Vector2Field("World Pos", startPos);
        endPos = EditorGUILayout.Vector2Field("Definition", endPos);

        GUILayout.EndArea();
    }
    public override float GetStageEditorGUIHeight(Rect rect)
    {
        return EditorGUIUtility.singleLineHeight * 3;
    }

    void DrawEnemyDataGizmos()
    {
        Vector2 screenPos = WorldToScreenPos(data.selectedEnemy.startPos);

        Handles.color = Color.cyan;

        Handles.DrawWireDisc(screenPos, Vector3.forward, 10);

        Vector2 definitionScreenPos = WorldToScreenPos(data.selectedEnemy.endPos);

        Handles.DrawLine(screenPos, definitionScreenPos);
        Vector2 X = new Vector2(setting.definitionGizmoSize, -setting.definitionGizmoSize);
        Handles.DrawLine(definitionScreenPos + X, definitionScreenPos - X);
        Handles.DrawLine(definitionScreenPos + Vector2.one * setting.definitionGizmoSize, definitionScreenPos - Vector2.one * setting.definitionGizmoSize);

    }
}
