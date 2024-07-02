using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using JetBrains.Annotations;

public static class CustomGUI
{
    static Event e => Event.current;

    public static void DrawSquare(Rect rect, SquareColor color)
    {
        Handles.DrawSolidRectangleWithOutline(rect, color.face, color.outline);
    }
    public static void DrawSquare(Rect rect, Color color)
    {
        Handles.DrawSolidRectangleWithOutline(rect, color, color);
    }
    public static void DrawOpenGrid(Rect rect, Vector2 offset, float cellSize, Color color)
    {
        Handles.color = color;

        for (float x = rect.x + offset.x; x < rect.x + rect.width; x += cellSize)
        {
            Vector3 p1 = new Vector3(x, rect.y);
            Vector3 p2 = new Vector3(x, rect.y + rect.height);

            Handles.DrawLine(p1, p2);
        }
        for (float y = rect.y + offset.y; y < rect.y + rect.height; y += cellSize)
        {
            Vector3 p1 = new Vector3(rect.x, y);
            Vector3 p2 = new Vector3(rect.x + rect.width, y);

            Handles.DrawLine(p1, p2);
        }
    }
    public static void DrawCloseGrid(Vector2 pos, Vector2Int cellCount, float cellSize, Color color)
    {
        Handles.color = color;
        Vector2 end = pos + cellSize * (Vector2)cellCount;
        for (int i = 0; i <= cellCount.x; i++)
        {
            float x = pos.x + i * cellSize;
            Vector3 p1 = new Vector3(x, pos.y);
            Vector3 p2 = new Vector3(x, end.y);

            Handles.DrawLine(p1, p2);
        }
        for (int i = 0; i <= cellCount.y; i++)
        {
            float y = pos.y + i * cellSize;
            Vector3 p1 = new Vector3(pos.x, y);
            Vector3 p2 = new Vector3(end.x, y);

            Handles.DrawLine(p1, p2);
        }
    }
    public static ObjectT InteractionObjectField<ObjectT>(Rect rect, ObjectT obj, Action interaction, bool allowSceneObject) where ObjectT : UnityEngine.Object
    {
        interaction.Invoke();

        return (ObjectT)EditorGUI.ObjectField(rect, obj, typeof(ObjectT), allowSceneObject);
    }
}
public static class CustmGUILayout
{
    static Event e => Event.current;
    public static ObjectT InteractionObjectField<ObjectT>(ObjectT obj, Action<Rect> interaction, float width = -1, float height = -1) where ObjectT : UnityEngine.Object
    {
        if (width == -1) width = GUILayoutUtility.GetLastRect().width;
        if (height == -1) height = EditorStyles.objectField.lineHeight;
        Rect rect = GUILayoutUtility.GetRect(width, height);

        interaction.Invoke(rect);

        return (ObjectT)EditorGUI.ObjectField(rect, obj, typeof(ObjectT), false);
    }
    public static void PreventMouseObjectField<ObjectT>(ObjectT obj, float width = -1, float height = -1) where ObjectT : UnityEngine.Object
    {
        if (width == -1) width = GUILayoutUtility.GetLastRect().width;
        if (height == -1) height = EditorStyles.objectField.lineHeight;
        Rect rect = GUILayoutUtility.GetRect(width, height);

        if (e.isMouse && rect.Contains(e.mousePosition)) e.Use();

        EditorGUI.ObjectField(rect, obj, typeof(ObjectT), false);
    }
}
