using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    public float time;
    protected Action endEvent;

    public void Execute(float time)
    {
        this.time += time;

        OnStart();
    }
    public virtual void OnStart() { }
    public virtual void OnUpdate() { }
    public virtual void OnEnd() { }
}
