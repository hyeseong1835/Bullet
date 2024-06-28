using UnityEngine;

[CreateAssetMenu(fileName = "AllKillWave", menuName = "Wave/AllKillWave", order = 0)]
public class AllKillWave : Wave
{
    protected override bool Trigger() => (Game.enemyList.Count <= 0);
}