


public abstract class UpgradableEffect : Effect
{
    public int level;
    public abstract UpgradableEffect GetVariant(int level);
    public abstract UpgradableEffect SetVariant(int level, UpgradableEffect value);
    public abstract void AddVariant(UpgradableEffect value);
    public abstract int GetVariantCount();

    public virtual void Upgrade()
    {
        OnEnd();
        UpgradableEffect effect = GetVariant(level + 1);
        effect.Execute(time);
        gameObject.SetActive(false);
    }
    public override void Execute(float time)
    {
        for (int i = GetVariantCount() - 1; i >= 0; i--)
        {
            UpgradableEffect variant = GetVariant(i);
            if (variant.gameObject.activeInHierarchy)
            {
                if (i > level)
                {
                    break;
                }
                else if (i == level)
                {
                    time += variant.time;
                    break;
                }
                else if (i < level)
                {
                    variant.Stop();
                    break;
                }
            }
        }
        base.Execute(time);
    }
    public void Init()
    {
        while (GetVariantCount() - 1 < level)
        {
            AddVariant(null);
        }
        SetVariant(level, this);
    }
}
