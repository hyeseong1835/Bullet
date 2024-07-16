using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

[Serializable]
public class Pool
{
    public GameObject prefab;

    public Transform holder { get; private set; }

    public int maxDisableCount;
    public int startCount;

    /// <summary>
    /// 앞: 활성화된 오브젝트 // 뒤: 비활성화된 오브젝트
    /// </summary>
    public List<GameObject> objects;

    [SerializeField] Action<GameObject> addEvent;

    public Pool(GameObject prefab, int maxDisableCount, int startCount)
    {
        this.prefab = prefab;

        this.startCount = startCount;

        this.maxDisableCount = maxDisableCount;
    }

    public void Init(Action<GameObject> addEvent = null)
    {
        this.addEvent = addEvent;

        if (holder != null) UnityEngine.Object.Destroy(holder.gameObject);

        holder = new GameObject(prefab.name).transform;
        holder.SetParent(PoolHolder.instance.transform);

        if (objects == null) objects = new List<GameObject>();
        else if (objects.Count != 0)
        {
            foreach (GameObject obj in objects)
            {
                if (obj != null) UnityEngine.Object.Destroy(obj);
            }
            objects.Clear();
        }

        for (int i = 0; i < startCount; i++)
        {
            Add();
        }

        PoolHolder.instance.pools.Add(this);
    }
    /// <summary>
    /// 활성화는 항상 Use를 이용해서 하기
    /// </summary>
    /// <returns>비활성화된 오브젝트</returns>
    public GameObject Get()
    {
        if (objects.Count == 0)
        {
            return Add();
        }

        GameObject obj = objects[^1];

        if (obj == null)
        {
            Debug.LogWarning($"Pool({prefab.name}) has null element({objects.Count - 1})");

            objects.RemoveAt(objects.Count - 1);
            return Get();
        }

        if (obj.activeInHierarchy)
        {
            return Add();
        }
        else
        {
            return obj;
        }
    }
    public GameObject Use()
    {
        if (objects.Count == 0) return Use(0, Get());
        else return Use(objects.Count - 1, Get());
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

        objects.Remove(obj);
        UnityEngine.Object.Destroy(obj);
        return null;
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