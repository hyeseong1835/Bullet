using System;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

public static class CustomGUILayout
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
    public static void UnderBarTitleText(string label)
    {
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        Rect labelRect = GUILayoutUtility.GetLastRect();
        float lineY = labelRect.position.y + EditorStyles.boldLabel.lineHeight + 3;
        Vector3 p1 = new Vector3(labelRect.x, lineY);
        Vector3 p2 = new Vector3(labelRect.x + labelRect.width, lineY);
        Handles.DrawLine(p1, p2);
    }
    public static void WarningLabel(string message, float topSpace = 10, float bottomSpace = 10)
    {
        EditorGUILayout.Space(topSpace);
        EditorGUILayout.LabelField(message, EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.Space(bottomSpace);
    }
    public static void TitleHeaderLabel(string title, float space = 5)
    {
        EditorGUILayout.Space(space);
        CustomGUILayout.UnderBarTitleText(title);
    }
    public static void BeginNewTab(float space = 10)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(space, false);
        EditorGUILayout.BeginVertical();
    }
    public static void EndNewTab()
    {
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
}
