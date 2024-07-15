using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradableEffect : Effect
{
    public EffectUpgradeData upgrade;
    public int level;

    public void Upgrade()
    {
        upgrade.variant[level + 1].Execute(time);
        time = 0;
    }
}
