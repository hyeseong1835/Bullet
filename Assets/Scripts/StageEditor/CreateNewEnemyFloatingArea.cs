#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public class CreateNewEnemyFloatingArea : FloatingArea
{
    Rect nameFieldRect = default;
    Rect enemyTypePopupRect = default;
    Rect createButtonRect = default;
    
    FloatingAreaManager _manager;
    //string name = "";

    GameObject selectPrefab;
    int arrayIndex = -1;
    int elementIndex = -1;

    public override void OnCreated()
    {
        _manager = new FloatingAreaManager();
    }
    public override float GetHeight()
    {
        return 100;
    }

    public override void EventListen(Event e)
    {
        _manager.EventListen(e);

        if (EventUtility.MouseDown(0))
        {
            if (nameFieldRect != default && nameFieldRect.Contains(e.mousePosition))
            {
                FocusNameField();
                e.Use();
            }
            else if (enemyTypePopupRect != default && enemyTypePopupRect.Contains(e.mousePosition))
            {
                ToggleArea();
                e.Use();
            }
            else if (createButtonRect != default 
                //&& name != ""
                && selectPrefab != null
                && createButtonRect.Contains(e.mousePosition))
            {
                Create();
                e.Use();
            }
        }

        
        
        void FocusNameField()
        {
            EditorGUI.FocusTextInControl("NameField");
        }   
        void ToggleArea()
        {
            if (_manager.area == null)
            {
                StageEditor.data.RefreshPrefab();

                _manager.Create(
                    new CategoryObjectFloatingArea(
                        StageEditor.data.enemyTypeNameArray,
                        StageEditor.data.prefabDoubleArray,
                        (i1, i2) => {
                            arrayIndex = i1;
                            elementIndex = i2;
                            selectPrefab = StageEditor.data.prefabDoubleArray[i1][i2];
                            _manager.area = null;
                        }
                    )
                );
            }
            else
            {
                enemyTypePopupRect = default;
                _manager.area = null;
            }
        }
        void Create()
        {
            string enemyTypeName = StageEditor.data.enemyTypeNameArray[arrayIndex];
            Type spawnDataType = StageEditor.data.GetEnemySpawnDataType(enemyTypeName);

            EnemySpawnData spawnData = (EnemySpawnData)ScriptableObject.CreateInstance(spawnDataType);
            spawnData.prefabTypeIndex = arrayIndex;
            spawnData.prefabIndex = elementIndex;

            if (StageEditor.data.selectedEnemyData != null)
            {
                spawnData.spawnTime = StageEditor.data.selectedEnemyData.SpawnData.spawnTime;
                StageEditor.data.timeFoldout[spawnData.spawnTime] = true;
            }

            EnemyEditorData editorEnemyData = spawnData.CreateEditorData();
            StageEditor.data.InsertToEditorEnemySpawnDataList(editorEnemyData);
            
            StageEditor.instance.Repaint();
            manager.area = null;
        }
    }

    public override void Draw()
    {
        GUILayout.BeginArea(manager.rect);
        {
            CustomGUILayout.TitleHeaderLabel("Create New Enemy");
            
            DrawNameField();
            DrawEnemyTypePopup();
            if (//name != "" && 
                selectPrefab != null
            )
            {
                DrawCreateButton();
            }
        }
        GUILayout.EndArea();
        
        _manager.Draw();

        void DrawNameField()
        {
            GUI.SetNextControlName("NameField");
            //name = EditorGUILayout.TextField(
              //  "Name", 
                //name
            //);
            
            if (Event.current.type == EventType.Repaint)
            {
                nameFieldRect = GUILayoutUtility.GetLastRect().AddPos(manager.rect.position);
            }
        }
        void DrawEnemyTypePopup()
        {
            EditorGUILayout.DropdownButton(
                new GUIContent(
                    selectPrefab?.name ?? "None"
                ),
                FocusType.Passive
            );
            if (Event.current.type == EventType.Repaint)
            {
                enemyTypePopupRect = GUILayoutUtility.GetLastRect().AddPos(manager.rect.position);
                _manager.SetRect(enemyTypePopupRect);
            }
        }

        void DrawCreateButton()
        {
            GUILayout.Box(
                "Create",
                GUI.skin.button
            );

            if (Event.current.type == EventType.Repaint)
            {
                createButtonRect = GUILayoutUtility.GetLastRect().AddPos(manager.rect.position);
            }
        }
    }
}
#endif