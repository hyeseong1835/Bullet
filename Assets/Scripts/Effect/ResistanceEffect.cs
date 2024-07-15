using System.Collections.Generic;

public class ResistanceEffect : UpgradableEffect
{
    static Player player => Player.instance;
    static List<ResistanceEffect> variant;
    public override int GetVariantCount() => variant.Count;
    public override UpgradableEffect GetVariant(int level) => variant[level];
    public override UpgradableEffect SetVariant(int level, UpgradableEffect value) => variant[level] = (ResistanceEffect)value;
    public override void AddVariant(UpgradableEffect value) => variant.Add((ResistanceEffect)value);

    public override void OnStart()
    {
        switch (level)
        {
            case 0:
                player.resistance = 0.5f;
                break;
            case 1:
                player.resistance = 0.25f;
                break;
            case 2:
                player.resistance = 0;
                break;
        }
    }
    public override void OnEnd()
    {
        player.resistance = 1;
        gameObject.SetActive(false);
    }
}
