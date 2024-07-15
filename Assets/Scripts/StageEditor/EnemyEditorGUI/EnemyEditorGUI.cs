#if UNITY_EDITOR
using UnityEditor;

public abstract class EnemyEditorGUI
{
    public virtual void OnSelected(EditorEnemyData enemyData) { }
    public virtual void OnDeSelected(EditorEnemyData enemyData) { }

    /// <summary>
    /// 에디터의 이벤트를 받기 전에 호출됩니다.
    /// </summary>
    public virtual void Event() { }
    /// <summary>
    /// 에디터의 이벤트를 받은 후 호출됩니다.
    /// </summary>
    public virtual void LateEvent() { }

    public virtual void DrawInspectorGUI(EditorEnemyData enemyData) { }
    public virtual void DrawSelectedEnemyDataGizmos(EditorEnemyData enemyData) { }
    public virtual void DrawSameTimeEnemyDataGizmos(EditorEnemyData enemyData) { }
    public virtual void Render(PreviewRenderUtility renderer, EditorEnemyData enemyData) { }
}
#endif