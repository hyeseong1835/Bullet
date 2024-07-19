
public class WeaponItem : Item
{
    public WeaponItemData data;
    public override ItemData ItemData {
        get => data;
        set { data = (WeaponItemData)value; }
    }

    protected override void OnPickup()
    {
        if (Player.instance.weapon == Player.instance.defaultWeapon) //�⺻ ����
        {
            Weapon weaponInstance = Instantiate(data.upgrade.data[0].prefab, Player.weaponHolder).GetComponent<Weapon>();
            Player.instance.GetWeapon(weaponInstance);
        }
        else if (data.upgrade == null) //���׷��̵� ���� �ʴ� ����
        {
            return;
        }
        else if (data.upgrade == Player.instance.weapon.data.upgrade) //������ ������ ����
        {
            int nextLevel = Player.instance.weapon.data.level + 1;
            if (nextLevel >= data.upgrade.data.Length) //�ִ� ����
            {
                return;
            }
            else Player.instance.UpgradeWeapon(); //���׷��̵�!
        }
        else return; //�ٸ� ������ ����

        ItemData.pool.DeUse(gameObject); //������ ���
    }
}
