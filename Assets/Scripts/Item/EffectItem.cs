
public class EffectItem : Item
{
    public enum StackType
    {
        Ignore, Replace, Add
    }
    
    public EffectItemData data;
    public override ItemData ItemData
    {
        get => data;
        set => data = (EffectItemData)value;
    }

    protected override bool OnPickup()
    {
        if (data.effect == null) data.effect = Player.instance.effects[data.effectIndex];

        switch (data.type)
        {
            case StackType.Ignore:
                if (data.effect.time != 0) return false;

                data.effect.Execute(data.time);
                return true;


            case StackType.Replace:
                if (data.effect.time < data.time)
                {
                    data.effect.Execute(data.time);
                    return true;
                }
                return false;

            case StackType.Add:
                if (data.effect.time != 0)
                {
                    data.effect.time += data.time;
                    data.effect.maxTime = data.effect.time;
                }
                else data.effect.Execute(data.time);
                return true;

            default: return false;
        }
    }
}
