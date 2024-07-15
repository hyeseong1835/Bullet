using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect Upgrade Data", menuName = "Effect Upgrade Data")]
public class EffectUpgradeData : ScriptableObject
{
    public List<UpgradableEffect> variant;
}
