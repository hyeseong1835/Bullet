using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Effect : MonoBehaviour
{
    public float maxTime = 1;
    public float time;
    protected Action endEvent;
    public Image timeFill;

    protected void Update()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            timeFill.fillAmount = time / maxTime;

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
        maxTime = this.time;
        gameObject.SetActive(true);
        OnStart();
    }
    public void Stop()
    {
        time = 0;
        OnEnd();
        gameObject.SetActive(false);
    }
    public virtual void OnStart() { }
    public virtual void OnUpdate() { }
    public virtual void OnEnd() { }
}
