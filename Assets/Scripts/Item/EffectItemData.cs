using UnityEngine;

[CreateAssetMenu(fileName = "Effect Item Data", menuName = "Data/Item/Effect")]
public class EffectItemData : ItemData
{
    public int effectIndex = 0;
    public Effect effect;
    public float time;
    public EffectItem.StackType type;
}
