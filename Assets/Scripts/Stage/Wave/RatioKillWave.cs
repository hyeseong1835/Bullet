using UnityEngine;

[CreateAssetMenu(fileName = "RatioKillWave", menuName = "Wave/RatioKillWave", order = 0)]
public class RatioKillWave : Wave
{
    [SerializeField] float ratio;
    protected override bool Trigger() => (Game.enemyList.Count / spawns.Length < ratio);
}