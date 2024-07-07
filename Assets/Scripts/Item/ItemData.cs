using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : ScriptableObject
{
    public Pool pool;

    public Item Drop()
    {
        if (pool.holder == null) pool.Init();

        Item item = pool.Use().GetComponent<Item>();
        item.ItemData = this;
        
        return item;
    }
}
