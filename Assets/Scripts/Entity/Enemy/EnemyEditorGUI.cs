#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class EnemyEditorGUI
{
    public virtual void DrawFiledViewerGUI()
    {
        
    }
    public virtual void Refresh()
    {

    }

    public virtual void OnSelected(EditorEnemyData enemyData)
    {
        if (enemyData.preview != null) Object.DestroyImmediate(enemyData.preview);

        if (enemyData.prefab == null)
        {
            Debug.LogError("Prefab is null");
            return;
        }

        enemyData.preview = StageEditor.instance.previewRender.InstantiatePrefabInScene(enemyData.prefab);
    }

    public virtual void OnDeSelected(EditorEnemyData enemyData)
    {
        if (enemyData.preview != null) Object.DestroyImmediate(enemyData.preview);
    }
    /// <summary>
    /// �������� �̺�Ʈ�� �ޱ� ���� ȣ��˴ϴ�.
    /// </summary>
    public virtual void Event() { }
    /// <summary>
    /// �������� �̺�Ʈ�� ���� �� ȣ��˴ϴ�.
    /// </summary>
    public virtual void LateEvent() { }

    public virtual void DrawInspectorGUI(EditorEnemyData enemyData) { }
    public virtual void DrawSelectedEnemyDataGizmos(EditorEnemyData enemyData) { }
    public virtual void DrawSameTimeEnemyDataGizmos(EditorEnemyData enemyData) { }
    public virtual void Render(EditorEnemyData enemyData) { }
}
#endif