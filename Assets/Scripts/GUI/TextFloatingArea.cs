#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public class TextFloatingArea : FloatingArea
{
    public string text;
    public string title;
    public Action<string> changeEvent;
    public Action<string> applyEvent;
    public Rect textFieldRect = default;

    public TextFloatingArea(string text, Action<string> changeEvent = null, Action<string> applyEvent = null, string title = "")
    {
        this.text = text;
        this.changeEvent = changeEvent;
        this.applyEvent = applyEvent;
        this.title = title;
    }
    public override void EventListen(Event e)
    {
        if (textFieldRect != default && EventUtility.MouseDown(0) && textFieldRect.Contains(e.mousePosition))
        {
            EditorGUI.FocusTextInControl("TextField");
        }
        if (EventUtility.KeyDown(KeyCode.Return) && GUI.GetNameOfFocusedControl() == "TextField")
        {
            GUI.FocusControl(null);
            applyEvent(text);
            manager.Destroy();
        }
    }
    public override void Draw()
    {
        GUILayout.BeginArea(manager.rect.GetAddSize(-Vector2.one * 5, RectUtility.Anchor.MiddleCenter));
        {
            if (title != "") CustomGUILayout.UnderBarTitleText(title);

            GUI.SetNextControlName("TextField");
            string input = GUILayout.TextField(text);
            if (Event.current.type == EventType.Repaint)
            {
                textFieldRect = GUILayoutUtility.GetLastRect().GetAddPos(manager.rect.position);
            }
            if (input != text)
            {
                changeEvent?.Invoke(input);
                text = input;
            }
        }
        GUILayout.EndArea();
    }

    public override float GetHeight()
    {
        if (title == "") return 30;
        else return 50;
    }
}
#endif