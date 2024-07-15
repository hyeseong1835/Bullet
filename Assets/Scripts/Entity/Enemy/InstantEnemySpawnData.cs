using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InstantEnemy SpawnData", menuName = "Data/Enemy/SpawnData/InstantEnemy")]
public class InstantEnemySpawnData : EnemySpawnData
{
    public override Type EditorType => typeof(InstantEnemyEditorGUI);
    public Vector2 startPos, endPos;
}
