#if UNITY_EDITOR
using UnityEditor;

public abstract class EnemyEditorGUI
{
    public virtual void OnSelected(EditorEnemyData enemyData) { }
    public virtual void OnDeSelected(EditorEnemyData enemyData) { }

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
    public virtual void Render(PreviewRenderUtility renderer, EditorEnemyData enemyData) { }
}
#endif