#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public abstract class EnemyEditorData
{
    public abstract EnemySpawnData SpawnData { get; protected set; }
    public abstract Type EnemyType { get; }

    public GameObject prefab { get; protected set; }

    public GameObject preview;

    public virtual void Apply()
    {
        EditorUtility.SetDirty(SpawnData);
    }

    public GameObject SelectPrefab(int index)
    {
        if (index == -1)
        {
            SpawnData.prefabIndex = -1;
            prefab = null;

            return null;
        }
        if (SpawnData.prefabTypeIndex < 0 
            || StageEditor.data.prefabDoubleArray.Length <= SpawnData.prefabTypeIndex)
        {
            Debug.LogError(
                $"Can't Select Prefab (Out of Index({SpawnData.prefabTypeIndex}) [0 ~ {StageEditor.data.prefabDoubleArray.Length - 1}])"
            );
            
            return SelectPrefab(-1);
        }
        GameObject[] prefabArray = StageEditor.data.prefabDoubleArray[SpawnData.prefabTypeIndex];
        if (prefabArray == null)
        {
            Debug.LogError(
               $"Can't Select Prefab (PrefabArray[{SpawnData.prefabTypeIndex}] is Null)"
           );

            return SelectPrefab(-1);
        }

        SpawnData.prefabIndex = index;
        return prefab = prefabArray[index];
    }

    public virtual void Refresh()
    {

    }

    public virtual void OnSelected()
    {
        if (preview != null) UnityEngine.Object.DestroyImmediate(preview);

        if (prefab == null)
        {
            Debug.LogError("Prefab is null");
            return;
        }

        preview = StageEditor.instance.previewRender.InstantiatePrefabInScene(prefab);
    }

    public virtual void OnDeSelected()
    {
        if (preview != null)
        {
            UnityEngine.Object.DestroyImmediate(preview);
            preview = null;
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

    public virtual void DrawInspectorGUI() { }
    public virtual void DrawSelectedEnemyDataGizmos() { }
    public virtual void DrawSameTimeEnemyDataGizmos() { }
    public virtual void Render() { }
}
#endif


