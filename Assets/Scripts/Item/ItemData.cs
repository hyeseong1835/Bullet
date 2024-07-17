using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : ScriptableObject
{
    public Pool pool;

    public Item Drop(Vector2 pos)
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
        Item item = pool.Get().GetComponent<Item>();
        item.transform.position = pos;
        pool.Use(pool.objects.Count - 1, item.gameObject);
        return item;
    }
}
