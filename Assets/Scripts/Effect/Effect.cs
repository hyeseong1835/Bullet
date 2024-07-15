using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    public float time;
    protected Action endEvent;

    protected void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            if (time <= 0)
            {
                OnEnd();
                return;
            }
        }
        OnUpdate();
    }
    public virtual void Execute(float time)
    {
        this.time += time;

        OnStart();
    }
    public void Stop()
    {
        time = 0;
        OnEnd();
    }
    public virtual void OnStart() { }
    public virtual void OnUpdate() { }
    public virtual void OnEnd() { }
}
