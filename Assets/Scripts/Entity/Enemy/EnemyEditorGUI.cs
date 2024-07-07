public abstract class EnemyEditorGUI
{

    /// <summary>
    /// �������� �̺�Ʈ�� �ޱ� ���� ȣ��˴ϴ�.
    /// </summary>
    public virtual void Event() { }
    /// <summary>
    /// �������� �̺�Ʈ�� ���� �� ȣ��˴ϴ�.
    /// </summary>
    public virtual void LateEvent() { }
    public abstract void DrawInspectorGUI(EditorEnemyData enemyData);
    public abstract void DrawEnemyDataGizmos(EditorEnemyData enemyData);
}