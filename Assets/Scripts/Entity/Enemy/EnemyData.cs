using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DropInfo
{
    public GameObject[] prefabs;
    public ItemData[] items;

    public float ratio;
}
public abstract class EnemyData : ScriptableObject
{
    public float maxHp;

    public List<DropInfo> drops = new List<DropInfo>();
    public float ratioMax;
    public float exp;

    void OnValidate()
    {
        ratioMax = 0;

        foreach (DropInfo drop in drops)
        {
            ratioMax += drop.ratio;
        }
    }
}
