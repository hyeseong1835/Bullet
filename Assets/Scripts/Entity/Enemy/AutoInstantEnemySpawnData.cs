using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InstantEnemy SpawnData", menuName = "Data/Enemy/SpawnData/InstantEnemy")]
public class AutoInstantEnemySpawnData : EnemySpawnData
{
#if UNITY_EDITOR
    public override EnemyEditorData CreateEditorData() => new AutoInstantEnemyEditorData(this);
#endif

    public Vector2 startPos;

    public override EnemySpawnData Copy()
    {
        AutoInstantEnemySpawnData copy = CreateInstance<AutoInstantEnemySpawnData>();
        copy.prefabIndex = prefabIndex;
        copy.prefabTypeIndex = prefabTypeIndex;
        copy.spawnTime = spawnTime;
        copy.startPos = startPos;

        return copy;
    }
}
