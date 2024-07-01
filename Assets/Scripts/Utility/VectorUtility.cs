using UnityEngine;

public static class VectorUtility
{
    public static Vector2 SetX(ref this Vector2 vector, float value) => vector = new Vector2(value, vector.y);
    public static Vector2 SetY(ref this Vector2 vector, float value) => vector = new Vector2(vector.x, value);

    public static Vector2 GetSetX(this Vector2 vector, float value) => vector.SetX(value);
    public static Vector2 GetSetY(this Vector2 vector, float value) => vector.SetY(value);

    public static Vector2 AddX(ref this Vector2 vector, float value) => vector.SetX(vector.x + value);
    public static Vector2 AddY(ref this Vector2 vector, float value) => vector.SetY(vector.y + value);

    public static Vector2 GetAddX(this Vector2 vector, float value) => vector.AddX(value);
    public static Vector2 GetAddY(this Vector2 vector, float value) => vector.AddY(value);

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
