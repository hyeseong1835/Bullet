using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RayUtility
{
    public static Ray2D AddOriginX(this Ray2D ray, float x)
    {
        return new Ray2D(ray.origin.GetAddX(x), ray.direction);
    }
    public static Ray2D AddOriginY(this Ray2D ray, float y)
    {
        return new Ray2D(ray.origin.GetAddY(y), ray.direction);
    }
    public static Ray2D AddOrigin(this Ray2D ray, Vector2 v)
    {
        return new Ray2D(ray.origin + v, ray.direction);
    }
    public static Ray AddOriginX(this Ray ray, float x)
    {
        return new Ray(ray.origin.GetAddX(x), ray.direction);
    }
    public static Ray AddOriginY(this Ray ray, float y)
    {
        return new Ray(ray.origin.GetAddY(y), ray.direction);
    }
    public static Ray AddOriginZ(this Ray ray, float z)
    {
        return new Ray(ray.origin.GetAddZ(z), ray.direction);
    }
    public static Ray AddOrigin(this Ray ray, Vector3 v)
    {
        return new Ray(ray.origin + v, ray.direction);
    }
}
