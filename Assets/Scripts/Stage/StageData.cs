using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageData : ScriptableObject
{
    [Serializable]
    public class EnemySpawnData
    {

    }

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

    public struct WaveData
    {
        public EnemySpawnData[] spawnDataArray;
        public WaveTrigger trigger;

        public WaveData(EnemySpawnData[] spawnDataArray, WaveTrigger trigger)
        {
            this.spawnDataArray = spawnDataArray;
            this.trigger = trigger;
        }
    }
    public WaveData[] waveDataArray;
    public int waveIndex = 0;
}

