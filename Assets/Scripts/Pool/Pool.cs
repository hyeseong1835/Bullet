using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum PoolType
{
    EnemyBullet,
    PlayerBullet,
    Enemy,
    Item
}

[Serializable]
public class Pool : IDisposable
{
    public PoolType type;

    public GameObject prefab;

    public Transform holder { get; private set; }

    public int maxDisableCount;
    public int startCount;

    /// <summary>
    /// 앞: 활성화된 오브젝트 // 뒤: 비활성화된 오브젝트
    /// </summary>
    public List<GameObject> objects { get; private set; }

    Action<GameObject> addEvent;

    public Pool()
    {
        if (holder == null)
        {
            if (prefab != null)
            {
                holder = new GameObject(prefab.name).transform;
                holder.SetParent(PoolHolder.instance.transform);
            }
        }
    }
    public Pool(PoolType type, GameObject prefab, int maxDisableCount, int startCount, Action<GameObject> addEvent = null)
    {
        this.type = type;

        this.prefab = prefab;

        this.startCount = startCount;

        this.maxDisableCount = maxDisableCount;
    
        holder = new GameObject(prefab.name).transform;
        holder.SetParent(PoolHolder.instance.transform);

        this.addEvent = addEvent;

        Clear();

        for (int i = 0; i < startCount; i++)
        {
            Add();
        }

        PoolHolder.instance.pools.Add(this);
        switch (type)
        {
            case PoolType.EnemyBullet: PoolHolder.instance.enemyBulletPools.Add(this); break;
            case PoolType.PlayerBullet: PoolHolder.instance.playerBulletPools.Add(this); break;
            case PoolType.Enemy: PoolHolder.instance.enemyPools.Add(this); break;
            case PoolType.Item: PoolHolder.instance.itemPools.Add(this); break;
        }
    }
    ~Pool()
    {
        Dispose();
    }

    public void Dispose()
    {
        Clear();

        if (holder != null) UnityEngine.Object.Destroy(holder.gameObject);
        addEvent = null;
    }
    public void Clear()
    {
        if (objects == null) objects = new List<GameObject>();
        else if (objects.Count != 0)
        {
            foreach (GameObject obj in objects)
            {
                if (obj != null) UnityEngine.Object.Destroy(obj);
            }
            objects.Clear();
        }
    }
    public void DeUseAll()
    {
        foreach (GameObject obj in objects)
        {
            obj?.SetActive(false);
        }
    }
    /// <summary>
    /// 활성화는 항상 Use를 이용해서 하기
    /// </summary>
    /// <returns>비활성화된 오브젝트</returns>
    public GameObject Get()
    {
        for (int i = objects.Count - 1; i >= 0; i--)
        {
            GameObject obj = objects[i];
            if (obj == null)
            {
                Debug.LogWarning($"Pool({prefab.name}) has null element({objects.Count - 1})");

                objects.RemoveAt(objects.Count - 1);
                continue;
            }

            if (obj.activeInHierarchy == false)
            {
                return obj;
            }
        }

        return Add();
    }
    public GameObject Use()
    {
        GameObject obj = Get();
        return Use(objects.Count - 1, obj);
    }
    public GameObject Use(GameObject obj) => Use(objects.LastIndexOf(obj), obj);
    public GameObject Use(int index) => Use(index, objects[index]);
    public GameObject Use(int index, GameObject obj)
    {
        objects.RemoveAt(index);
        objects.Insert(0, obj);

        obj.SetActive(true);
        return obj;
    }
    public GameObject DeUse(GameObject obj)
    {
        obj.SetActive(false);
        objects.Remove(obj);
        objects.Add(obj);

        return obj;
    }
    public GameObject DeUse(GameObject obj, int index)
    {
        obj.SetActive(false);
        objects.RemoveAt(index);
        objects.Add(obj);

        return obj;
    }
    public GameObject Add()
    {
        GameObject obj = UnityEngine.Object.Instantiate(prefab, holder);
        obj.name = holder.name;
        obj.SetActive(false);

        objects.Add(obj);
        addEvent?.Invoke(obj);
        
        return obj;
    }
}