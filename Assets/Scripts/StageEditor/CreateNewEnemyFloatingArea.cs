using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CreateNewEnemyFloatingArea : FloatingArea
{
    int selectedTypeIndex = -1;
    Rect enemyTypePopupRect;
    FloatingAreaManager _manager;
    string[] selectableEnemyTypeArray;

    public override void OnCreated()
    {
        base.OnCreated();
        _manager = new FloatingAreaManager();
    }
    public override float GetHeight()
    {
        return 100;
    }

    public override void EventListen(Event e)
    {
        _manager.EventListen(e);

        if (enemyTypePopupRect != default)
        {
            CustomGUI.DrawSquare(enemyTypePopupRect, Color.white);
            if (EventUtility.MouseDown(0) && enemyTypePopupRect.Contains(e.mousePosition))
            {
                if(_manager.area == null)
                {
                    _manager.Create(new TextPopupFloatingArea(selectableEnemyTypeArray, Select));
                }
                else
                {
                    _manager.Destroy();
                }
                e.Use();
            }
        }
    }
    void Select(int index)
    {
        selectedTypeIndex = index;
    }

    public override void Draw()
    {
        GUILayout.BeginArea(manager.rect);
        {
            CustomGUILayout.TitleHeaderLabel("Create New Enemy");
            DrawEnemyTypePopup();
        }
        GUILayout.EndArea();

        _manager.Draw();



        void DrawEnemyTypePopup()
        {
            selectableEnemyTypeArray = new string[StageEditor.data.prefabTypeList.Count + 1];
            selectableEnemyTypeArray[0] = "None";

            for (int i = 1; i < selectableEnemyTypeArray.Length; i++)
            {
                selectableEnemyTypeArray[i] = StageEditor.data.prefabTypeList[i - 1].Name;
            }
            string dropDownLabel;
            if (selectedTypeIndex == -1)
            {
                dropDownLabel = "None";
            }
            else
            {
                dropDownLabel = selectableEnemyTypeArray[selectedTypeIndex];
            }
            EditorGUILayout.DropdownButton(new GUIContent(dropDownLabel), FocusType.Passive);
            if (Event.current.type == EventType.Repaint)
            {
                enemyTypePopupRect = GUILayoutUtility.GetLastRect().GetAddPos(manager.rect.position);
                _manager.SetRect(enemyTypePopupRect);
            }
        }
    }
}
