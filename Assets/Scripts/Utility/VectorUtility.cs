using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorUtility
{
    public static Vector2 Bezier(Vector2 startPos, Vector2 endPos, Vector2 handlePos, float t)
    {
        Vector2 a = Vector2.Lerp(startPos, handlePos, t);
        Vector2 b = Vector2.Lerp(handlePos, endPos, t);

        return Vector2.Lerp(a, b, t);
    }
    public static Vector3 Bezier(Vector3 startPos, Vector3 endPos, Vector3 handlePos, float t)
    {
        Vector3 a = Vector3.Lerp(startPos, handlePos, t);
        Vector3 b = Vector3.Lerp(handlePos, endPos, t);

        return Vector3.Lerp(a, b, t);
    }
}
