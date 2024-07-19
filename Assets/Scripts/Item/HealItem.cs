using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItem : Item
{
    public HealItemData data;
    public override ItemData ItemData
    {
        get => data;
        set { data = (HealItemData)value; }
    }

    Player player => Player.instance;


    protected override bool OnPickup()
    {
        player.Heal(data.healAmount);
        return true;
    }
}
