using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "Data/Stage")]
public class Stage : ScriptableObject
{
    public Pool[] enemyPool;
    public int waveIndex = 0;

    public void Start()
    {
        foreach (Pool pool in enemyPool)
        {
            if (pool.holder == null) pool.Init();
        }
    }
}

