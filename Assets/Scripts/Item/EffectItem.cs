using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    protected override void OnPickup()
    {
        switch (data.type)
        {
            case StackType.Ignore:
                if (data.effect.time == 0)
                {
                    data.effect.Execute(data.time);

                }
                break;
            case StackType.Replace:
                data.effect.Execute(data.time);
                break;
            case StackType.Add:
                data.effect.Execute(data.time);
                break;
        }
    }
}
