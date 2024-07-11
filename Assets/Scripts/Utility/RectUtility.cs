using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class RectUtility
{
    public static Vector2 GetCenter(this Rect rect) => rect.position + 0.5f * rect.size;

    #region Set

    #region Position
    public static Rect SetPos(ref this Rect rect, Vector2 position)
    {
        rect.position = position;
        return rect;
    }
    public static Rect SetX(ref this Rect rect, float x)
    {
        rect.x = x;
        return rect;
    }
    public static Rect SetY(ref this Rect rect, float y)
    {
        rect.y = y;
        return rect;
    }

    public static Rect GetSetPos(this Rect rect, Vector2 position) => rect.SetPos(position);
    public static Rect GetSetX(this Rect rect, float x) => rect.SetX(x);
    public static Rect GetSetY(this Rect rect, float y) => rect.SetY(y);

    #endregion

    #region Size
    public static Rect SetSize(ref this Rect rect, Vector2 size)
    {
        rect.size = size;
        return rect;
    }
    public static Rect SetWidth(ref this Rect rect, float width)
    {
        rect.width = width;
        return rect;
    }
    public static Rect SetHeight(ref this Rect rect, float height)
    {
        rect.height = height;
        return rect;
    }

    public static Rect GetSetSize(this Rect rect, Vector2 size) => rect.SetSize(size);
    public static Rect GetSetWidth(this Rect rect, float width) => rect.SetWidth(width);
    public static Rect GetSetHeight(this Rect rect, float height) => rect.SetHeight(height);

    #endregion

    #endregion

    #region Add

    public static Rect AddPos(ref this Rect rect, Vector2 position)
    {
        rect.position += position;
        return rect;
    }
    public static Rect AddX(ref this Rect rect, float x)
    {
        rect.x += x;
        return rect;
    }
    public static Rect AddY(ref this Rect rect, float y)
    {
        rect.y += y;
        return rect;
    }

    public static Rect GetAddPos(this Rect rect, Vector2 position) => rect.AddPos(position);
    public static Rect GetAddX(this Rect rect, float x) => rect.AddX(x);
    public static Rect GetAddY(this Rect rect, float y) => rect.AddY(y);

    public static Rect AddSize(ref this Rect rect, Vector2 size)
    {
        rect.size += size;
        return rect;
    }
    public static Rect AddWidth(ref this Rect rect, float width)
    {
        rect.width += width;
        return rect;
    }
    public static Rect AddHeight(ref this Rect rect, float height)
    {
        rect.height += height;
        return rect;
    }
    public static Rect GetAddSize(this Rect rect, Vector2 size) => rect.AddSize(size);
    public static Rect GetAddWidth(this Rect rect, float width) => rect.AddWidth(width);
    public static Rect GetAddHeight(this Rect rect, float height) => rect.AddHeight(height);

    #endregion
}
