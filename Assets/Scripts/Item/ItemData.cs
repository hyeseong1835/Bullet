using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : ScriptableObject
{
    public Pool pool;

    public Item Drop()
    {
        if (pool.holder == null)
        {
            pool.Init(
                (obj) => 
                {
                    Item item = obj.GetComponent<Item>();
                    item.ItemData = this;
                }
            );
        }
        
        return pool.Use().GetComponent<Item>();
    }
}
