using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class Pusher : MonoBehaviour
{
    const int detail = 10;

    const float startPosGizmosSize = 0.1f;
    const float endPosGizmosSize = 0.1f;
    const float handlePosGizmosSize = 0.1f;
    
    Entity entity;

    public PusherData data;
    

    private void Start()
    {
        StartCoroutine(Push());
    }
    IEnumerator Push()
    {
        entity = GetComponent<Entity>();
        entity.state = EntityState.Push;
        yield return new WaitForSeconds(data.startTime);
        
        float time = data.startTime;
        while (time != data.endTime)
        {
            time += Time.deltaTime;
            if (time > data.endTime) time = data.endTime;

            float t = data.moveCurve.Evaluate(time);
            transform.position = Bezier(t);

            yield return null;
        }
        entity.state = EntityState.Enable;
        Destroy(this);
    }
    Vector2 Bezier(float t)
    {
        Vector2 a = Vector2.Lerp(data.startPos, data.handlePos, t);
        Vector2 b = Vector2.Lerp(data.handlePos, data.endPos, t);

        return Vector2.Lerp(a, b, t);
    }
    private void OnDrawGizmosSelected()
    {
        if (data == null) return;

        Gizmos.color = Color.blue;
       
        Gizmos.DrawWireSphere(data.startPos, startPosGizmosSize);
        
        Vector2 two = new Vector2(1, -1);
        Gizmos.DrawLine(data.endPos + Vector2.one * endPosGizmosSize, data.endPos - Vector2.one * endPosGizmosSize);
        Gizmos.DrawLine(data.endPos + two * endPosGizmosSize, data.endPos - two * endPosGizmosSize);

        Gizmos.DrawWireCube(data.handlePos, Vector2.one * handlePosGizmosSize);

        Vector2 prevPos = data.startPos;

        for (int i = 1; i <= detail; i++)
        {
            Vector2 curPos = Bezier((float)i / detail);

            Gizmos.DrawLine(prevPos, curPos);

            prevPos = curPos;
        }
    }
    private void OnValidate()
    {
        if (data == null) return;

        transform.position = new Vector3(data.startPos.x, data.startPos.y, transform.position.z);

        if (data.moveCurve.length == 0)
        {
            data.moveCurve.AddKey(0, 0);
            data.moveCurve.AddKey(1, 1);
        }
        foreach(Keyframe key in data.moveCurve.keys)
        {
            if (key.value > 1)
            {
                Debug.LogWarning("MoveCurve의 값은 1보다 작거나 같아야 합니다.");
            }
        }
        data.startTime = data.moveCurve.keys[0].time;
        data.endTime = data.moveCurve.keys[^1].time;
    }
}
