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
        if (Player.instance.weapon == null)
        {
            Weapon weaponInstance = Instantiate(data.weaponPrefab, Player.weaponHolder).GetComponent<Weapon>();
            Player.instance.weapon = weaponInstance;
            Player.instance.weaponUI.sprite = weaponInstance.UI;


            ItemData.pool.DeUse(gameObject);
        }

    }
}
