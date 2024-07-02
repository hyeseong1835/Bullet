using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class RectUtility
{
    #region Set

    #region Position
    public static Rect SetPosition(ref this Rect rect, Vector2 position)
    {
        rect.position = position;
        return rect;
    }
    public static Rect SetPostionX(ref this Rect rect, float x)
    {
        rect.x = x;
        return rect;
    }
    public static Rect SetPostionY(ref this Rect rect, float y)
    {
        rect.y = y;
        return rect;
    }

    public static Rect GetSetPosition(this Rect rect, Vector2 position) => rect.SetPosition(position);
    public static Rect GetSetPostionX(this Rect rect, float x) => rect.SetPostionX(x);
    public static Rect GetSetPostionY(this Rect rect, float y) => rect.SetPostionY(y);

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

    public static Rect AddPosition(ref this Rect rect, Vector2 position)
    {
        rect.position += position;
        return rect;
    }
    public static Rect AddPositionX(ref this Rect rect, float x)
    {
        rect.x += x;
        return rect;
    }
    public static Rect AddPositionY(ref this Rect rect, float y)
    {
        rect.y += y;
        return rect;
    }

    public static Rect GetAddPosition(this Rect rect, Vector2 position) => rect.AddPosition(position);
    public static Rect GetAddPositionX(this Rect rect, float x) => rect.AddPositionX(x);
    public static Rect GetAddPositionY(this Rect rect, float y) => rect.AddPositionY(y);

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
