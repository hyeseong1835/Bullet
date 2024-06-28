using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("WaveTrigger/RemainEnemyWaveTrigger")]
public class RemainEnemyWaveTrigger : WaveTrigger
{
    [SerializeField] int remainEnemyCount;
    
    void Update()
    {
        if (GameManager.instance.enableEnemyList.Count <= remainEnemyCount)
        {
            wave.SetActive(true);
            Destroy(this);
        }
    }
}
