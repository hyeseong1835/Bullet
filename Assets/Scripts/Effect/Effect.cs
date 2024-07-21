using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class Effect : MonoBehaviour
{
    public Image timeFill;

    [NonSerialized] public float maxTime = 0;
    public float time;

    protected void Update()
    {
        if (maxTime == -1)
        {
            OnUpdate();
        }
        else
        {
            if (Time()) OnUpdate();
            else Stop();
        }
        
    }
    bool Time()
    {
        if (time != 0)
        {
            time -= GameManager.deltaTime;
            if (time < 0) time = 0;

            timeFill.fillAmount = 1 - time / maxTime;
        }

        return time != 0;
    }
    public virtual void Execute(float time)
    {
        if (time == -1)
        {
            this.time = 0;
            maxTime = -1;
            timeFill.fillAmount = 0;
        }
        else
        {
            this.time = time;
            if (maxTime < this.time) maxTime = this.time;
        }

        gameObject.SetActive(true);
        OnStart();
    }
    public void Stop()
    {
        time = 0;
        maxTime = 0;
        OnEnd();
        gameObject.SetActive(false);
    }
    public virtual void OnStart() { }
    public virtual void OnUpdate() { }
    public virtual void OnEnd() { }
}
