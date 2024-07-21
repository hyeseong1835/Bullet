using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class PoolHolder : MonoBehaviour
{
    public static PoolHolder instance;

    public List<Pool> pools = new List<Pool>();
    public List<Pool> enemyPools = new List<Pool>();
    public List<Pool> enemyBulletPools = new List<Pool>();
    public List<Pool> playerBulletPools = new List<Pool>();

    public List<Pool> itemPools = new List<Pool>();

    void Awake() => instance = this;

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
                foreach (Pool pool in pools)
                {
                    pool.Dispose();
                }
                pools.Clear();
                enemyPools.Clear();
                enemyBulletPools.Clear();
                playerBulletPools.Clear();
                itemPools.Clear();

                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }
                break;
        }
    }
#endif
    void OnValidate()
    {
        instance = this;
    }
}
