using System;

[Serializable]
public class InventoryItem: Item
{
    public InventoryItemData data;

    protected override void OnPickup()
    {
        inventory.AddItem(data);
    }
}
