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

    [SerializeField] Vector2 startPos;
    [SerializeField] Vector2 endPos;
    [SerializeField] Vector2 handlePos;

    [SerializeField] AnimationCurve moveCurve;
    [SerializeField] float startTime;
    [SerializeField] float endTime;


    private void Start()
    {
        StartCoroutine(Push());
    }
    IEnumerator Push()
    {
        entity = GetComponent<Entity>();
        entity.state = EntityState.Push;
        yield return new WaitForSeconds(startTime);
        
        float time = startTime;
        while (time != endTime)
        {
            time += Time.deltaTime;
            if (time > endTime) time = endTime;

            float t = moveCurve.Evaluate(time);
            transform.position = Bezier(t);

            yield return null;
        }
        entity.state = EntityState.Enable;
        Destroy(this);
    }
    Vector2 Bezier(float t)
    {
        Vector2 a = Vector2.Lerp(startPos, handlePos, t);
        Vector2 b = Vector2.Lerp(handlePos, endPos, t);

        return Vector2.Lerp(a, b, t);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
       
        Gizmos.DrawWireSphere(startPos, startPosGizmosSize);
        
        Vector2 two = new Vector2(1, -1);
        Gizmos.DrawLine(endPos + Vector2.one * endPosGizmosSize, endPos - Vector2.one * endPosGizmosSize);
        Gizmos.DrawLine(endPos + two * endPosGizmosSize, endPos - two * endPosGizmosSize);

        Gizmos.DrawWireCube(handlePos, Vector2.one * handlePosGizmosSize);

        Vector2 prevPos = startPos;

        for (int i = 1; i <= detail; i++)
        {
            Vector2 curPos = Bezier((float)i / detail);

            Gizmos.DrawLine(prevPos, curPos);

            prevPos = curPos;
        }
    }
    private void OnValidate()
    {
        transform.position = new Vector3(startPos.x, startPos.y, transform.position.z);

        if (moveCurve.length == 0)
        {
            moveCurve.AddKey(0, 0);
            moveCurve.AddKey(1, 1);
        }
        foreach(Keyframe key in moveCurve.keys)
        {
            if (key.value > 1)
            {
                Debug.LogWarning("MoveCurve의 값은 1보다 작거나 같아야 합니다.");
            }
        }
        startTime = moveCurve.keys[0].time;
        endTime = moveCurve.keys[^1].time;
    }
}
