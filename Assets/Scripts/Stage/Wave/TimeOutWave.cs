using UnityEngine;

[CreateAssetMenu(fileName = "TimeOutWave", menuName = "Wave/TimeOutWave", order = 0)]
public class TimeOutWave : Wave
{
    public float time;
    float curTime;

    protected override bool Trigger() => (curTime <= 0);
    protected override void Start()
    {
        curTime = time;
    }
    protected override void OnTime()
    {
        curTime -= Time.deltaTime;
    }
}