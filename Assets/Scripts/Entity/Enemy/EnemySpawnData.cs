using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Enemy Spawn", menuName = "Data/Enemy")]
public abstract class EnemySpawnData : ScriptableObject
{
    public abstract Type EditorType { get; }

    public int prefabIndex;
    public float spawnTime;
}