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
        pool.pool.Add(obj);
        pool.waitDestroy.Remove(this);
    }
}
[Serializable]
public class Pool
{
    public int count { get { return pool.Count; } }

    public GameObject prefab;

    public Transform holder { get; private set; }
    public float destroyDelay;
    
    public int startCount;
    public int stayCount;

    [HideInInspector] public List<GameObject> pool = new List<GameObject>();
    [HideInInspector] public List<WaitDestroyElement> waitDestroy = new List<WaitDestroyElement>();//-|

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
        foreach (GameObject obj in pool)
        {
            if (obj != null) UnityEngine.Object.Destroy(obj);
        }
        foreach (WaitDestroyElement element in waitDestroy)
        {
            if (element.obj != null) UnityEngine.Object.Destroy(element.obj);
        }
        pool.Clear();
        waitDestroy.Clear();
        for (int i = 0; i < startCount; i++)
        {
            Add().SetActive(false);
        }

        PoolHolder.instance.pools.Add(this);
    }
    public GameObject Get()
    {
        //비활성화 오브젝트 찾기
        foreach (GameObject gameObject in pool)
        {
            if (gameObject == null)
            {
                Debug.LogWarning("Pool has null element");
                pool.Remove(gameObject);
                continue;
            }
            if (gameObject.activeInHierarchy == false)
            {
                return gameObject;
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
            pool.Remove(obj);

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

        pool.Add(obj);
        if (addEvent != null) addEvent(obj);
        return obj;
    }
}