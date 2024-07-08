using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Enemy Spawn", menuName = "Data/Enemy")]
public abstract class EnemySpawnData : ScriptableObject
{
    public abstract Type EditorType { get; }

    public int prefabIndex = -1;
    public int prefabTypeIndex = -1;
    public float spawnTime;
}