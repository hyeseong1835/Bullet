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
    public abstract void DrawInspectorGUI(EnemySpawnData enemy);
    public abstract void DrawEnemyDataGizmos(EnemySpawnData enemy);
}