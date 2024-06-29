using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pusher Data", menuName = "Data/Pusher")]
public class PusherData : ScriptableObject
{
    public Vector2 startPos;
    public Vector2 endPos;
    public Vector2 handlePos;

    public AnimationCurve moveCurve;
    public float startTime;
    public float endTime;
}
