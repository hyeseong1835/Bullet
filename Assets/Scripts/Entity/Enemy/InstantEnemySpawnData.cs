using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InstantEnemy SpawnData", menuName = "Data/Enemy/SpawnData/InstantEnemy")]
public class InstantEnemySpawnData : EnemySpawnData
{
#if UNITY_EDITOR
    public override Type EditorType => typeof(InstantEnemyEditorGUI);
#endif
    public Vector2 startPos, endPos;
}
