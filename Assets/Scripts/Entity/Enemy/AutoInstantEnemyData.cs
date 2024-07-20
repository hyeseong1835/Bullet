using UnityEngine;

[CreateAssetMenu(fileName = "AutoInstantEnemy Data", menuName = "Data/Enemy/AutoInstantEnemy")]
public class AutoInstantEnemyData : EnemyData
{
    public float collideDamage = 1;
    public float speed;
}
