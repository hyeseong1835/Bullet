
public class WeaponItem : Item
{
    public WeaponItemData data;
    public override ItemData ItemData {
        get => data;
        set { data = (WeaponItemData)value; }
    }

    protected override void OnPickup()
    {
        if (Player.instance.weapon == Player.instance.defaultWeapon) //기본 무기
        {
            Weapon weaponInstance = Instantiate(data.upgrade.data[0].prefab, Player.weaponHolder).GetComponent<Weapon>();
            Player.instance.GetWeapon(weaponInstance);
        }
        else if (data.upgrade == null) //업그레이드 하지 않는 무기
        {
            return;
        }
        else if (data.upgrade == Player.instance.weapon.data.upgrade) //동일한 종류의 무기
        {
            int nextLevel = Player.instance.weapon.data.level + 1;
            if (nextLevel >= data.upgrade.data.Length) //최대 레벨
            {
                return;
            }
            else Player.instance.UpgradeWeapon(); //업그레이드!
        }
        else return; //다른 종류의 무기

        ItemData.pool.DeUse(gameObject); //아이템 사용
    }
}
