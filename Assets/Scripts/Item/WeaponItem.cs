
public class WeaponItem : Item
{
    public WeaponItemData data;
    public override ItemData ItemData {
        get => data;
        set { data = (WeaponItemData)value; }
    }

    protected override bool OnPickup()
    {
        if (Player.instance.weapon == Player.instance.defaultWeapon) //기본 무기
        {
            Weapon weaponInstance = Instantiate(
                data.upgrade.data[0].prefab, 
                Player.instance.weaponHolder
            ).GetComponent<Weapon>();

            Player.instance.GetWeapon(weaponInstance);
            return true;
        }
        if (data.upgrade == null) //업그레이드 하지 않는 무기
        {
            return false;
        }
        if (data.upgrade == Player.instance.weapon.data.upgrade) //동일한 종류의 무기
        {
            int nextLevel = Player.instance.weapon.data.level + 1;
            if (nextLevel >= data.upgrade.data.Length) //최대 레벨
            {
                return false;
            }
            else
            {
                Player.instance.UpgradeWeapon(); //업그레이드!
                return true;
            }
        }
        else //다른 종류의 무기
        {
            return false;
        }
    }
}
