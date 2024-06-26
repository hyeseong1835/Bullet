using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItem : Item
{
    [SerializeField] HealItemData data;

    protected override void OnPickup() => player.Heal(data.healAmount);
}
