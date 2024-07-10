using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FloatingAreaManager
{
    public FloatingArea area;
    public SquareColor backGroundColor;
    public Vector2 position;
    public float width;
    public Rect rect;
    bool created;

    public void SetRect(Vector2 position, float width)
    {
        this.position = position;
        this.width = width;
    }
    public void SetRect(Rect headerRect)
    {
        SetRect(headerRect.position.GetAddY(headerRect.height), headerRect.width);
    }
    public void EventListen(Event e)
    {
        area?.EventListen(e);
    }
    public void Create(FloatingArea area)
    {
        area.manager = this;
        this.area = area;

        created = true;
    }
    public void Draw()
    {
        if (area != null)
        {
            rect = new Rect(position, new Vector2(width, area.GetHeight()));

            if (created)
            {
                area.OnCreated();
                created = false;
            }
            area.CreateField();
            area.Draw();
        }
    }
    public void Destroy()
    {
        area = null;
    }
}

public abstract class FloatingArea
{
    public FloatingAreaManager manager;
    public abstract float GetHeight();

    public abstract void EventListen(Event e);

    public abstract void Draw();

    public virtual void CreateField()
    {
        GUI.SetNextControlName("FloatingArea");
        CustomGUI.DrawSquare(manager.rect, manager.backGroundColor);
    }
    public virtual void OnCreated()
    {
        GUI.FocusControl("FloatingArea");
    }
}
