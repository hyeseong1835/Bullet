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
    public Box(Collider2D coll)
    {
        size = coll.bounds.size;
        center = coll.offset;
    }
    public bool IsExitUp(float y, float line) => y < line + 0.5f * size.y + center.y;
    public bool IsExitDown(float y, float line) => y > line - 0.5f * size.y + center.y;
    public bool IsExitRight(float x, float line) => x < line + 0.5f * size.x + center.x;
    public bool IsExitLeft(float x, float line) => x > line - 0.5f * size.x + center.x;
    public bool IsExit(Vector2 pos, float up, float down, float right, float left)
    {
        return IsExitUp(pos.y, up) 
            || IsExitDown(pos.y, down) 
            || IsExitRight(pos.x, right) 
            || IsExitLeft(pos.x, left);
    }
    public bool IsExitGameUp(float x) => IsExitUp(x, GameCanvas.Up);
    public bool IsExitGameDown(float x) => IsExitDown(x, GameCanvas.Down);
    public bool IsExitGameRight(float y) => IsExitRight(y, GameCanvas.Right);
    public bool IsExitGameLeft(float y) => IsExitLeft(y, GameCanvas.Left);
    public bool IsExitGame(Vector2 pos)
    {
        return IsExitGameUp(pos.y)
            || IsExitGameDown(pos.y)
            || IsExitGameRight(pos.x)
            || IsExitGameLeft(pos.x);
    }
}