using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Data/Enemy")]
public class EnemySpawnData : ScriptableObject
{
    public int enemySpawnID;

    public Vector2 worldPos;
    public Vector2 definition;
    public float spawnTime;
}