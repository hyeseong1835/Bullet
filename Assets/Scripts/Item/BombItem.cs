public class BombItem : Item
{
    public BombItemData data;
    public override ItemData ItemData {
        get => data;
        set => data = (BombItemData)value;
    }

    protected override bool OnPickup()
    {
        foreach (Pool pool in PoolHolder.instance.enemyBulletPools)
        {
            pool.DeUseAll();
        }
        foreach (Pool pool in PoolHolder.instance.enemyPools)
        {
            pool.DeUseAll();
        }
        return true;
    }
}