using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaitDestroyElement
{
    PoolHolder coroutineRunner => PoolHolder.instance;

    public GameObject obj;
    
    Coroutine destroyCoroutine;

    public WaitDestroyElement(Pool pool, GameObject obj)
    {
        this.obj = obj;
        
        destroyCoroutine = coroutineRunner.StartCoroutine(DelayDestroy(pool, pool.destroyDelay));
    }
    IEnumerator DelayDestroy(Pool pool, float delay)
    {
        yield return new WaitForSeconds(delay);

        UnityEngine.Object.Destroy(obj);
        pool.waitDestroy.Remove(this);
    }
    public void CancelDestroy(Pool pool)
    {
        coroutineRunner.StopCoroutine(destroyCoroutine);
        pool.objects.Add(obj);
        pool.waitDestroy.Remove(this);
    }
}
[Serializable]
public class Pool
{
    public int count { get { return objects.Count; } }

    public GameObject prefab;

    public Transform holder { get; private set; }
    public float destroyDelay;
    
    public int startCount;
    public int stayCount;

    [HideInInspector] public List<GameObject> objects;
    [HideInInspector] public List<WaitDestroyElement> waitDestroy;//-|

    [SerializeField] Action<GameObject> addEvent;

    public Pool(GameObject prefab, float destroyDelay, int startCount, int stayCount)
    {
        this.prefab = prefab;
        this.destroyDelay = destroyDelay;
        this.startCount = startCount;
        this.stayCount = stayCount;
    }

    public void Init(Action<GameObject> addEvent = null)
    {
        this.addEvent = addEvent;

        if (holder == null)
        {
            holder = new GameObject(prefab.name).transform;
            holder.SetParent(PoolHolder.instance.transform);
        }
        if (objects == null) objects = new List<GameObject>();
        else if (objects.Count != 0)
        {
            foreach (GameObject obj in objects)
            {
                if (obj != null) UnityEngine.Object.Destroy(obj);
            }
            objects.Clear();
        }

        if (waitDestroy == null) waitDestroy = new List<WaitDestroyElement>();
        else if (waitDestroy.Count != 0)
        {
            foreach (WaitDestroyElement element in waitDestroy)
            { 
                if (element.obj != null) UnityEngine.Object.Destroy(element.obj);
            }
            waitDestroy.Clear();
        }
        
        for (int i = 0; i < startCount; i++)
        {
            Add().SetActive(false);
        }

        PoolHolder.instance.pools.Add(this);
    }
    public GameObject Get()
    {
        //비활성화 오브젝트 찾기
        for (int i = objects.Count - 1; i >= 0; i--)
        {
            GameObject obj = objects[i];
            if (obj == null)
            {
                Debug.LogError("Pool has null element");
                objects.RemoveAt(i);
                continue;
            }
            if (obj.activeInHierarchy == false)
            {
                return obj;
            }
        }
        //파괴 예정 오브젝트가 있을 때
        if (waitDestroy.Count > 0)
        {
            WaitDestroyElement element = waitDestroy[^1];
            GameObject obj = element.obj;
            if (obj == null)
            {
                Debug.LogWarning("WaitDestroy has null element");
                waitDestroy.RemoveAt(waitDestroy.Count - 1);
                return Use();
            }
            element.CancelDestroy(this);
            return obj;
        }
        //모두 사용 중일 때
        return Add();
    }
    public GameObject Use()
    {
        GameObject obj = Get();
        obj.SetActive(true);
        return obj;
    }
    public GameObject DeUse(GameObject obj)
    {
        //초과 상태일 때
        if (count > stayCount)
        {
            waitDestroy.Add(new WaitDestroyElement(this, obj));
            objects.Remove(obj);

            obj.SetActive(false);

            return null;//?
        }
        else
        {
            obj.SetActive(false);

            return obj;
        }
    }
    public GameObject Add()
    {
        GameObject obj = UnityEngine.Object.Instantiate(prefab, holder);
        obj.name = holder.name;
        obj.SetActive(false);

        objects.Add(obj);
        if (addEvent != null) addEvent(obj);
        return obj;
    }
}