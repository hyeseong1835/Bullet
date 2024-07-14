using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

public class CreateNewEnemyFloatingArea : FloatingArea
{
    Rect nameFieldRect = default;
    Rect enemyTypePopupRect = default;
    Rect createButtonRect = default;
    
    FloatingAreaManager _manager;
    string name = "";

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
                && name != ""
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
                StageEditor.data.RefreshPrefabList();

                string[] prefabTypeNameArray = new string[StageEditor.data.prefabTypeList.Count];
                for (int i = 0; i < StageEditor.data.prefabTypeList.Count; i++)
                {
                    prefabTypeNameArray[i] = StageEditor.data.prefabTypeList[i].Name;
                }

                _manager.Create(
                    new CategoryObjectFloatingArea(
                        prefabTypeNameArray,
                        StageEditor.data.prefabLists.ToDoubleArray(),
                        (i1, i2) => {
                            arrayIndex = i1;
                            elementIndex = i2;
                            selectPrefab = StageEditor.data.prefabLists[i1][i2];
                            _manager.Destroy();
                        }
                    )
                );
            }
            else
            {
                enemyTypePopupRect = default;
                _manager.Destroy();
            }
        }
        void Create()
        {
            Debug.Log($"Create: {arrayIndex}/{elementIndex}");
            Type prefabType = StageEditor.data.prefabTypeList[arrayIndex];

            EnemySpawnData spawnData = (EnemySpawnData)ScriptableObject.CreateInstance(prefabType);
            spawnData.prefabTypeIndex = arrayIndex;
            spawnData.prefabIndex = elementIndex;

            AssetDatabase.CreateAsset(
                spawnData,
                $"{StageEditor.data.GetStageDirectoryPath(StageEditor.data.selectedStage)}/EnemySpawnData/{name}.asset"
            );

            EditorEnemyData editorEnemyData = new EditorEnemyData(spawnData, prefabType);
            StageEditor.data.InsertToEditorEnemySpawnDataList(editorEnemyData);

            manager.Destroy();
        }
    }

    public override void Draw()
    {
        GUILayout.BeginArea(manager.rect);
        {
            CustomGUILayout.TitleHeaderLabel("Create New Enemy");
            
            DrawNameField();
            DrawEnemyTypePopup();
            if (name != ""
                && selectPrefab != null
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
            name = EditorGUILayout.TextField(
                "Name", 
                name
            );
            
            if (Event.current.type == EventType.Repaint)
            {
                nameFieldRect = GUILayoutUtility.GetLastRect().GetAddPos(manager.rect.position);
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
                enemyTypePopupRect = GUILayoutUtility.GetLastRect().GetAddPos(manager.rect.position);
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
                createButtonRect = GUILayoutUtility.GetLastRect().GetAddPos(manager.rect.position);
            }
        }
    }
}
