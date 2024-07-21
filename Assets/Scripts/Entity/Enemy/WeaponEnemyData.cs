using UnityEngine;

[CreateAssetMenu(fileName = "WeaponEnemy Data", menuName = "Data/Enemy/WeaponEnemy")]
public class WeaponEnemyData : EnemyData
{
    public float collideDamage = 1;
    public float speed;
}
