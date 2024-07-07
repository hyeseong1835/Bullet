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
    public List<Pool> bulletPools = new List<Pool>();
    public List<Pool> itemPools = new List<Pool>();

    void Awake() => instance = this;

    private void Update()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlaying == false)
        {
            pools.Clear();
            enemyPools.Clear();
            bulletPools.Clear();
            itemPools.Clear();

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
#endif
    }
    void OnValidate()
    {
        instance = this;
    }
}
