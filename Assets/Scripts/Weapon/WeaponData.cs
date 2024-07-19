using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Data/Weapon/Data")]
public class WeaponData : ScriptableObject
{
    [Header("Object")]
    public GameObject prefab;
    public Sprite UI;

    [Header("Upgrade")]
    public WeaponUpgradeData upgrade;
    public int level;
}