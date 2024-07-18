using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : Item
{
    public WeaponItemData data;
    public override ItemData ItemData {
        get => data;
        set { data = (WeaponItemData)value; }
    }

    protected override void OnPickup()
    {
        Weapon weaponInstance = Instantiate(data.weaponPrefab, Player.weaponHolder).GetComponent<Weapon>();

    }
}
