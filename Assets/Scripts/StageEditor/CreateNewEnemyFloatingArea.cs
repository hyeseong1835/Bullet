using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CreateNewEnemyFloatingArea : FloatingArea
{
    int selectedTypeIndex = -1;
    public override float GetHeight()
    {
        return 100;
    }

    public override void EventListen(Event e)
    {

    }

    public override void Draw()
    {
        GUILayout.BeginArea(manager.rect);
        {
            CustomGUILayout.TitleHeaderLabel("Create New Enemy");
            DrawEnemyTypePopup();
        }
        GUILayout.EndArea();

        void DrawEnemyTypePopup()
        {
            string[] selectableEnemyTypeArray = new string[StageEditor.data.prefabTypeList.Count + 1];
            selectableEnemyTypeArray[0] = "None";
            for (int i = 1; i < selectableEnemyTypeArray.Length; i++)
            {
                selectableEnemyTypeArray[i] = StageEditor.data.prefabTypeList[i - 1].Name;
            }
            
            int selectInput = EditorGUILayout.Popup(selectedTypeIndex + 1, selectableEnemyTypeArray);

            if (selectInput != 0)
            {
                selectedTypeIndex = selectInput - 1;
            }
        }
    }
}
