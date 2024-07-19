
public class WeaponItem : Item
{
    public WeaponItemData data;
    public override ItemData ItemData {
        get => data;
        set { data = (WeaponItemData)value; }
    }

    protected override bool OnPickup()
    {
        if (Player.instance.weapon == Player.instance.defaultWeapon) //�⺻ ����
        {
            Weapon weaponInstance = Instantiate(
                data.upgrade.data[0].prefab, 
                Player.instance.weaponHolder
            ).GetComponent<Weapon>();

            Player.instance.GetWeapon(weaponInstance);
            return true;
        }
        if (data.upgrade == null) //���׷��̵� ���� �ʴ� ����
        {
            return false;
        }
        if (data.upgrade == Player.instance.weapon.data.upgrade) //������ ������ ����
        {
            int nextLevel = Player.instance.weapon.data.level + 1;
            if (nextLevel >= data.upgrade.data.Length) //�ִ� ����
            {
                return false;
            }
            else
            {
                Player.instance.UpgradeWeapon(); //���׷��̵�!
                return true;
            }
        }
        else //�ٸ� ������ ����
        {
            return false;
        }
    }
}
