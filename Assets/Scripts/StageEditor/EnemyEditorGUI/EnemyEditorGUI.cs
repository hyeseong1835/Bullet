#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class EnemyEditorGUI
{
    public Dictionary<EditorEnemyData, GameObject> instance = new Dictionary<EditorEnemyData, GameObject>();

    public virtual void Refresh()
    {
        foreach (GameObject value in instance.Values)
        {
            Object.DestroyImmediate(value);
        }
        instance.Clear();
    }

    public virtual void OnSelected(EditorEnemyData enemyData)
    {
        GameObject obj;
        if (instance.TryGetValue(enemyData, out obj))
        {
            //Debug.LogWarning($"Can't Select (Key({enemyData.prefab.name}) already exist)");
            if (obj != null) Object.DestroyImmediate(obj);

            obj = StageEditor.instance.previewRender.InstantiatePrefabInScene(enemyData.prefab);
            instance[enemyData] = obj;
        }
        else
        {
            obj = StageEditor.instance.previewRender.InstantiatePrefabInScene(enemyData.prefab);
            instance.Add(enemyData, obj);
        }
        obj.transform.position = ((InstantEnemySpawnData)(enemyData.spawnData)).startPos;
    }

    public virtual void OnDeSelected(EditorEnemyData enemyData)
    {
        if (instance.TryGetValue(enemyData, out GameObject obj) == false)
        {
            //Debug.LogWarning($"Can't DeSelect (Key({enemyData.prefab.name}) not found)");
        }
        else
        {
            Object.DestroyImmediate(obj);
            instance.Remove(enemyData);
        }
    }

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