public abstract class EnemyEditorGUI
{

    /// <summary>
    /// 에디터의 이벤트를 받기 전에 호출됩니다.
    /// </summary>
    public virtual void Event() { }
    /// <summary>
    /// 에디터의 이벤트를 받은 후 호출됩니다.
    /// </summary>
    public virtual void LateEvent() { }
    public abstract void DrawInspectorGUI(EnemySpawnData enemy);
    public abstract void DrawEnemyDataGizmos(EnemySpawnData enemy);
}