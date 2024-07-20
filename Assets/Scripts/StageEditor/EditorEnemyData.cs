#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorEnemyData
{
    public EnemySpawnData spawnData { get; private set; }
    public EnemyEditorGUI editorGUI => unSafeEditorGUI ?? SetEditorGUI();
    public EnemyEditorGUI unSafeEditorGUI { get; private set; }

    public GameObject prefab { get; private set; }
    public Type prefabType { get; private set; }

    public GameObject preview;

    public EditorEnemyData(EnemySpawnData spawnData)
    {
        this.spawnData = spawnData;
        
        prefabType = StageEditor.data.prefabTypeList[spawnData.prefabTypeIndex];
        prefab = SelectPrefab(spawnData.prefabIndex);
    }
    public EditorEnemyData(EnemySpawnData spawnData, Type prefabType)
    {
        this.spawnData = spawnData;
        this.prefabType = prefabType;

        prefab = SelectPrefab(spawnData.prefabIndex);
    }
    public EditorEnemyData(EnemySpawnData spawnData, GameObject prefab, Type prefabType)
    {
        this.spawnData = spawnData;
        this.prefabType = prefabType;

        this.prefab = prefab;
    }
    public EditorEnemyData Copy()
    {
        return new EditorEnemyData(
            spawnData.Copy(),
            prefab,
            prefabType
        );
    }
    public void Apply()
    {
        spawnData.prefabTypeIndex = StageEditor.data.GetPrefabListIndex(spawnData);
        spawnData.prefabIndex = StageEditor.data.prefabLists[spawnData.prefabTypeIndex].IndexOf(prefab);

        spawnData.prefabIndex = StageEditor.data.GetPrefabList(prefabType).IndexOf(prefab);
        EditorUtility.SetDirty(spawnData);
    }

    #region EditorGUI

    public EnemyEditorGUI SetEditorGUI()
    {
        unSafeEditorGUI = StageEditor.instance.GetEnemyEditor(spawnData.EditorType);

        return unSafeEditorGUI;
    }

    #endregion

    #region Prefab

    public GameObject SelectPrefab(int index)
    {
        if (index == -1)
        {
            prefab = null;
            spawnData.prefabIndex = -1;

            return null;
        }
        List<GameObject> prefabList = StageEditor.data.GetPrefabList(prefabType);
        if (prefabList == null)
        {
            Debug.LogError("Can't Select Prefab (PrefabList is Null)");

            prefab = null;
            spawnData.prefabIndex = -1;

            return null;
        }

        if (0 <= index && index < prefabList.Count)
        {
            spawnData.prefabIndex = index;

            return prefab = prefabList[index];
        }
        else
        {
            if (prefabList.Count > 0)
            {
                Debug.LogWarning($"Can't Select Prefab (Out of Index {index}/{prefabList.Count - 1})");

                spawnData.prefabIndex = 0;
                return prefab = prefabList[0];
            }
            Debug.LogError("Can't Select Prefab (PrefabList is Empty)");

            prefab = null;
            spawnData.prefabIndex = -1;

            return null;
        }
    }

    #endregion
}
#endif