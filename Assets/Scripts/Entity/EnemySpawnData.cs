using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Spawn", menuName = "Data/Enemy")]
public class EnemySpawnData : ScriptableObject
{
    public GameObject enemyPrefab;
    public float spawnTime;
}