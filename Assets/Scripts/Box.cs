using System;
using UnityEngine;

[Serializable]
public struct Box
{
    public Vector2 size;
    public Vector2 center;

    public Box(Vector2 size, Vector2 center)
    {
        this.size = size;
        this.center = center;
    }
}