using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InstantEnemy SpawnData", menuName = "Data/Enemy/SpawnData/InstantEnemy")]
public class WeaponEnemySpawnData : EnemySpawnData
{
#if UNITY_EDITOR
    public override EnemyEditorData CreateEditorData() => new WeaponEnemyEditorData(this);
#endif

    public Vector2 startPos;

    public override EnemySpawnData Copy()
    {
        WeaponEnemySpawnData copy = CreateInstance<WeaponEnemySpawnData>();
        copy.prefabIndex = prefabIndex;
        copy.prefabTypeIndex = prefabTypeIndex;
        copy.spawnTime = spawnTime;
        copy.startPos = startPos;

        return copy;
    }
}
