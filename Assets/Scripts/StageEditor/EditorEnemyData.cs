using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorEnemyData
{
    public EnemySpawnData spawnData;
    public EnemyEditorGUI editorGUI => unSafeEditorGUI ?? SetEditorGUI();
    public EnemyEditorGUI unSafeEditorGUI { get; private set; }

    public GameObject prefab;
    public Type prefabType;

    public EditorEnemyData(EnemySpawnData spawnData, Type prefabType)
    {
        this.spawnData = spawnData;
        this.prefabType = prefabType;

        prefab = SelectPrefab(spawnData.prefabIndex);
    }
    public void Apply()
    {
        StageEditor.data.RefreshPrefabList();
        ApplyPrefab();

        EditorUtility.SetDirty(StageEditor.data);
    }

    #region EditorGUI

    public EnemyEditorGUI SetEditorGUI()
    {
        unSafeEditorGUI = StageEditor.data.GetEnemyEditor(spawnData.EditorType);

        return unSafeEditorGUI;
    }

    #endregion

    #region Prefab

    public void ApplyPrefab()
    {
        spawnData.prefabIndex = StageEditor.data.GetPrefabList(prefabType).IndexOf(prefab);
    }
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