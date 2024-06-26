using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryItemData : ScriptableObject
{
    public string itemName;
    public Sprite sprite;

    public Vector2Int position;
    public Vector2Int size;

    public abstract void Use();
}
