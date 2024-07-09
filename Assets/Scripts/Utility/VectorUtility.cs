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

    public static Vector2 MultiplyX(ref this Vector2 vector, float value) => vector.SetX(vector.x * value);
    public static Vector2 MultiplyY(ref this Vector2 vector, float value) => vector.SetY(vector.y * value);
    
    public static Vector2 GetMultiplyX(this Vector2 vector, float value) => vector.MultiplyX(value);
    public static Vector2 GetMultiplyY(this Vector2 vector, float value) => vector.MultiplyY(value);

    public static Vector3 SetX(ref this Vector3 vector, float value) => vector = new Vector3(value, vector.y, vector.z);
    public static Vector3 SetY(ref this Vector3 vector, float value) => vector = new Vector3(vector.x, value, vector.z);
    public static Vector3 SetZ(ref this Vector3 vector, float value) => vector = new Vector3(vector.x, vector.y, value);

    public static Vector3 GetSetX(this Vector3 vector, float value) => vector.SetX(value);
    public static Vector3 GetSetY(this Vector3 vector, float value) => vector.SetY(value);
    public static Vector3 GetSetZ(this Vector3 vector, float value) => vector.SetZ(value);

    public static Vector3 AddX(ref this Vector3 vector, float value) => vector.SetX(vector.x + value);
    public static Vector3 AddY(ref this Vector3 vector, float value) => vector.SetY(vector.y + value);
    public static Vector3 AddZ(ref this Vector3 vector, float value) => vector.SetZ(vector.z + value);

    public static Vector3 GetAddX(this Vector3 vector, float value) => vector.AddX(value);
    public static Vector3 GetAddY(this Vector3 vector, float value) => vector.AddY(value);
    public static Vector3 GetAddZ(this Vector3 vector, float value) => vector.AddZ(value);

    public static Vector3 MultiplyX(ref this Vector3 vector, float value) => vector.SetX(vector.x * value);
    public static Vector3 MultiplyY(ref this Vector3 vector, float value) => vector.SetY(vector.y * value);
    public static Vector3 MultiplyZ(ref this Vector3 vector, float value) => vector.SetZ(vector.z * value);

    public static Vector3 GetMultiplyX(this Vector3 vector, float value) => vector.MultiplyX(value);
    public static Vector3 GetMultiplyY(this Vector3 vector, float value) => vector.MultiplyY(value);
    public static Vector3 GetMultiplyZ(this Vector3 vector, float value) => vector.MultiplyZ(value);

    public static Vector2 Rotate(this Vector2 vector, float delta)
    {
        float sin = Mathf.Sin(delta);
        float cos = Mathf.Cos(delta);

        return new Vector2(
            vector.x * cos - vector.y * sin,
            vector.x * sin + vector.y * cos
        );
    }

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
