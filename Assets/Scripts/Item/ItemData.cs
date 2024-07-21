using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public Pool pool { get; protected set; } = null;
    public GameObject prefab;

#if UNITY_EDITOR
    void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnEditorPlayModeStateChanged;
    }
    void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnEditorPlayModeStateChanged;
    }
    void OnEditorPlayModeStateChanged(PlayModeStateChange state)
    {
        switch (state)
        {
            case PlayModeStateChange.ExitingPlayMode:
                pool = null;
                break;
        }
    }
#endif
    public virtual Item Drop(Vector2 pos, Vector2 velocity)
    {
        if (pool == null)
        {
            pool = new Pool(
                PoolType.Item,
                prefab,
                0,
                0,
                (obj) =>
                    {
                        Item item = obj.GetComponent<Item>();
                        item.ItemData = this;
                    }
            );
        }
        Item item = pool.Get().GetComponent<Item>();
        item.transform.position = pos;
        item.velocity = velocity;
        pool.Use(pool.objects.Count - 1, item.gameObject);
        return item;
    }
}
