using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "Data/Stage")]
public class Stage : ScriptableObject
{
    [Serializable]
    public class WaveTrigger
    {
        [SerializeField] int remainEnemyCount = -1;
        [SerializeField] float time = 0;
        bool triggerTime = false;
        bool triggerRemainEnemyCount => remainEnemyCount == -1 || GameManager.instance.enableEnemyList.Count <= remainEnemyCount;
        public void WaveStart()
        {
            if (time == 0) triggerTime = true;
            else GameManager.instance.Invoke(nameof(Timer), time);
        }
        public bool IsTrigger() => triggerTime && triggerRemainEnemyCount;

        void Timer() => triggerTime = true;
    }
    [Serializable]
    public struct WaveData
    {
        public EnemySpawnData[] spawnData;
        public WaveTrigger trigger;

        public WaveData(EnemySpawnData[] spawnData, WaveTrigger trigger)
        {
            this.spawnData = spawnData;
            this.trigger = trigger;
        }
    }
    public Pool[] enemyPool;
    public WaveData[] waveData;
    public int waveIndex = 0;

    public void Start()
    {
        foreach (Pool pool in enemyPool)
        {
            if (pool.holder == null) pool.Init();
        }
    }
}

