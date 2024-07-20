using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Enemy Spawn", menuName = "Data/Enemy")]
public abstract class EnemySpawnData : ScriptableObject
{
#if UNITY_EDITOR
    public abstract EnemyEditorData CreateEditorData();
#endif
    public int prefabIndex = -1;
    public int prefabTypeIndex = -1;
    public float spawnTime;

    public abstract EnemySpawnData Copy();
}