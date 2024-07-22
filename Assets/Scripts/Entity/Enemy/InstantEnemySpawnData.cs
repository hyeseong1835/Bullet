using UnityEngine;

[CreateAssetMenu(fileName = "InstantEnemy SpawnData", menuName = "Data/Enemy/SpawnData/InstantEnemy")]
public class InstantEnemySpawnData : EnemySpawnData
{
    public Vector2 startPos, endPos;

#if UNITY_EDITOR
    public override EnemyEditorData CreateEditorData() => new InstantEnemyEditorData(this);
#endif
    public override EnemySpawnData Copy()
    {
        InstantEnemySpawnData copy = CreateInstance<InstantEnemySpawnData>();
        copy.prefabIndex = prefabIndex;
        copy.prefabTypeIndex = prefabTypeIndex;
        copy.spawnTime = spawnTime;
        copy.startPos = startPos;
        copy.endPos = endPos;

        return copy;
    }
}
