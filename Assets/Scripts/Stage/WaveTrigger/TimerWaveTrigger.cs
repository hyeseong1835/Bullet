using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("WaveTrigger/TimerWaveTrigger")]
public class TimerWaveTrigger : WaveTrigger
{
    [SerializeField] float time;

    private void OnEnable()
    {
        Invoke("Timer", time);
    }
    void Timer() => wave.SetActive(true);
}
