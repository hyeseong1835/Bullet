using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Wave : ScriptableObject
{
    protected GameManager Game => GameManager.instance;

    [Serializable]
    public struct Spawn
    {
        public GameObject prefab;
        public Vector2 pos;
        public float speed;
        public float delay;
    }
    public Spawn[] spawns;
    public float spawnDelay;
    protected abstract bool Trigger();
    protected virtual void Start() { }
    protected virtual void OnTime() { }
}