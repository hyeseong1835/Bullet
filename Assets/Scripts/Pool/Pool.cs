using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaitDestroyElement
{
    PoolHolder coroutineRunner => PoolHolder.instance;

    Pool pool;
    public GameObject obj;
    
    Coroutine destroyCoroutine;

    public WaitDestroyElement(Pool pool, GameObject obj)
    {
        this.pool = pool;
        this.obj = obj;
        
        destroyCoroutine = coroutineRunner.StartCoroutine(DelayDestroy());
    }
    IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(pool.destroyDelay);

        UnityEngine.Object.Destroy(obj);
        pool.waitDestroy.Remove(this);
    }
    public void CancelDestroy()
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

    Transform holder;
    public float destroyDelay;
    
    public int startCount;
    public int stayCount;

    [HideInInspector] public List<GameObject> pool = new List<GameObject>();
    [HideInInspector] public List<WaitDestroyElement> waitDestroy = new List<WaitDestroyElement>();//-|

    [SerializeField] Action<GameObject> addEvent;

    public void Init(Action<GameObject> addEvent)
    {
        this.addEvent = addEvent;

        holder = new GameObject(prefab.name).transform;
        holder.SetParent(PoolHolder.instance.transform);

        for (int i = 0; i < startCount; i++)
        {
            Add().SetActive(false);
        }
    }

    public GameObject Use()
    {
        //비활성화 오브젝트 찾기
        foreach (GameObject gameObject in pool)
        {
            if (gameObject == null)
            {
                Debug.Log("pool 공란 제거");
                pool.Remove(gameObject);
                continue;
            }
            if (gameObject.activeInHierarchy == false)
            {
                gameObject.SetActive(true);
                return gameObject;
            }
        }
        //파괴 예정 오브젝트가 있을 때
        if (waitDestroy.Count > 0)
        {
            WaitDestroyElement element = waitDestroy[0];
            GameObject obj = element.obj;
            if (obj == null)
            {
                Debug.Log("waitDestroy 공란 제거");
                return Use();
            }
            obj.SetActive(true);
            element.CancelDestroy();
            return obj;
        }
        //모두 사용 중일 때
        return Add();
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
        addEvent(obj);
        pool.Add(obj);
        return obj;
    }
}