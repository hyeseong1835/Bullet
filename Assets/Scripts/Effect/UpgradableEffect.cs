


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
        effect.gameObject.SetActive(true);
        effect.Execute(time);
        gameObject.SetActive(false);
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
