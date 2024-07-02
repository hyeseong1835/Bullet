using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

public class StageEditor : EditorWindow
{
    public static StageEditorData data;
    public static StageEditorSetting setting;
    
    static Event e => Event.current;

    static Dictionary<Type, EnemyEditorGUI> enemyEditorGUIDictionary = new Dictionary<Type, EnemyEditorGUI>();

    Rect fileViewerRect = new Rect();
    Rect inspectorRect = new Rect();
    Rect previewRect = new Rect();

    bool holdInspectorLine = false;
    bool holdFilesLine = false;

    float timeLength = 100;

    string openStageFolderPath = "";

    bool isPrefabDropDownExpanded = false;
    Vector2 offset;
    
    Action drawDropDown;
    
    int dropDownIndex = 0;
    Rect dropDownRect;


    [MenuItem("Window/StageEditor")]
    static void CreateWindow()
    {
        StageEditor window = (StageEditor)EditorWindow.GetWindow(typeof(StageEditor));

        window.Show();
    }
    void OnGUI()
    {

        Init();

        
        //PreviewMove();

        DrawPreview();


        LineMove();
        
        DrawFileViewerGUI();
        DrawInspectorGUI();

        
        if (data.selectedEnemySpawnData != null)
        {
            DrawTimeLine();
        }

        #region Function

        void Init()
        {
            LoadData();

            void LoadData()
            {
                if (data == null) data = (StageEditorData)EditorResources.Load<ScriptableObject>("StageEditor/Data.asset");
                if (setting == null) setting = (StageEditorSetting)EditorResources.Load<ScriptableObject>("StageEditor/Setting.asset");

                if (data.selectedEnemySpawnData != null && data.selectedEnemySpawnData.enemyPrefab != null && data.selectedEnemySpawnData != null)
                {
                    data.selectedEnemyEditorGUI = GetEnemyEditor();
                }
            }
        }
        

        void PreviewMove()
        {
            if (e.button == 0)
            {
                switch (e.type)
                {
                    case EventType.MouseDown:
                        if (previewRect.GetAddWidth(setting.lineHoldWidth).GetAddPositionX(0.5f * setting.lineHoldWidth).Contains(e.mousePosition))
                        {
                            offset = data.previewPos - e.mousePosition;
                            e.Use();
                        }
                        break;

                    case EventType.MouseDrag:
                        data.previewPos = e.mousePosition + offset;
                        if (previewRect.GetAddWidth(setting.lineHoldWidth).GetAddPositionX(0.5f * setting.lineHoldWidth).Contains(e.mousePosition))
                        {
                            e.Use();
                            if (data.preview.IsContact(data.previewPos, position.height, 0, data.inspectorLinePosX, data.filesLinePosX, out Vector2 previewContact))
                            {
                                data.previewPos = previewContact;
                            }
                            Repaint();
                        }
                        break;
                    }
            }
        }

        void DrawPreview()
        {
            DrawGrid();

            if (data.selectedEnemySpawnData != null && data.selectedEnemyEditorGUI != null)
            {
                data.selectedEnemyEditorGUI.DrawEnemyDataGizmos(data.selectedEnemySpawnData);
            }

            void DrawGrid()
            {
                CustomGUI.DrawSquare(previewRect, setting.previewBackGroundColor);
                CustomGUI.DrawOpenGrid(previewRect, new Vector2(data.previewPos.x % data.cellSize, data.previewPos.y % data.cellSize), data.cellSize, setting.previewOutGridColor);
                CustomGUI.DrawCloseGrid(data.previewPos + data.cellSize * new Vector2(-0.5f * Window.GameWidth, -Window.GameHeight), new Vector2Int(Window.GameWidth, Window.GameHeight), data.cellSize, setting.previewGameGridColor);
            }
        }

        void DrawTimeLine()
        {
            float timeLineX = data.filesLinePosX + setting.timeHorizontalSpace;
            float timeLineY = position.height - setting.timeBottomSpace;
            float timeLineWidth = data.inspectorLinePosX - 2 * setting.timeHorizontalSpace;

            Handles.color = Color.white;
            Handles.DrawLine(
                    new Vector3(data.filesLinePosX + setting.timeHorizontalSpace, timeLineY),
                    new Vector3(data.inspectorLinePosX - setting.timeHorizontalSpace, timeLineY)
                );

            for (int i = 0; i < data.editorEnemyDataList.Count; i++)
            {
                if (i == data.selectedEnemyIndex)
                {
                    float timeRatio = data.selectedEnemySpawnData.spawnTime / timeLength;
                    Rect timeLineMarkerRect = new Rect();
                    timeLineMarkerRect.position = new Vector2(timeLineX + timeRatio * timeLineWidth, timeLineY);
                    timeLineMarkerRect.size = Vector2.one * setting.timeCubeSize;

                    CustomGUI.DrawSquare(timeLineMarkerRect, setting.selectBoxColor);
                }
                else
                {
                    EnemySpawnData spawnData = data.editorEnemyDataList[i];
                    Handles.DrawWireCube(
                        new Vector3(data.filesLinePosX + setting.timeHorizontalSpace + (spawnData.spawnTime / timeLength) * (data.inspectorLinePosX - 2 * setting.timeHorizontalSpace), timeLineY),
                        Vector3.one * setting.timeCubeSize
                    );
                }
            }

        }
        

        void LineMove()
        {
            FileViewerLineMove();
            InspectorLineMove();

            if (EventUtility.MouseUp(0))
            {
                holdFilesLine = false;
                holdInspectorLine = false;
            }

            void FileViewerLineMove()
            {
                if (holdInspectorLine) return;

                if (EventUtility.MouseDown(0))
                {
                    float distance = e.mousePosition.x - data.filesLinePosX;
                    if (Mathf.Abs(distance) < setting.lineHoldWidth)
                    {
                        holdFilesLine = true;
                        e.Use();
                    }
                }
                if (holdFilesLine && EventUtility.MouseDrag(0))
                {
                    data.filesLinePosX = e.mousePosition.x;

                    float inspectorLineMin = data.filesLinePosX + setting.minDistanceBetweenLines;
                    if (data.inspectorLinePosX < inspectorLineMin)
                    {
                        data.inspectorLinePosX = inspectorLineMin;
                        RefreshInspectorRect();
                    }
                    RefreshFileViewerRect();
                    RefreshPreviewRect();
                    Repaint();
                    e.Use();
                }

                if (data.filesLinePosX < 0)
                {
                    data.filesLinePosX = 0;
                    RefreshFileViewerRect();
                    RefreshPreviewRect();
                    Repaint();
                }
            }

            void InspectorLineMove()
            {
                if (holdFilesLine) return;

                if (EventUtility.MouseDown(0))
                {
                    float distance = e.mousePosition.x - data.inspectorLinePosX;
                    if (Mathf.Abs(distance) < setting.lineHoldWidth)
                    {
                        holdInspectorLine = true;
                        e.Use();
                    }
                }
                if (holdInspectorLine && EventUtility.MouseDrag(0))
                {
                    data.inspectorLinePosX = e.mousePosition.x;
                    float fileLineMax = data.inspectorLinePosX - setting.minDistanceBetweenLines;
                    if (data.filesLinePosX > fileLineMax)
                    {
                        data.filesLinePosX = fileLineMax;
                        RefreshFileViewerRect();
                    }
                    RefreshInspectorRect();
                    RefreshPreviewRect();
                    Repaint();
                    e.Use();
                }
                if (data.inspectorLinePosX > position.width)
                {
                    data.inspectorLinePosX = position.width;
                    RefreshInspectorRect();
                    RefreshPreviewRect();
                    Repaint();
                }
            }


            void RefreshFileViewerRect()
            {
                fileViewerRect.position = new Vector2(0, 0);
                fileViewerRect.size = new Vector2(data.filesLinePosX, position.height);
            }
            void RefreshInspectorRect()
            {
                inspectorRect.position = new Vector2(data.inspectorLinePosX, 0);
                inspectorRect.size = new Vector2(position.width - data.inspectorLinePosX, position.height);
            }
            void RefreshPreviewRect()
            {
                previewRect.position = new Vector2(data.filesLinePosX, 0);
                previewRect.size = new Vector2(data.inspectorLinePosX - data.filesLinePosX, position.height);
            }
        }

        void DrawFileViewerGUI()
        {
            CustomGUI.DrawSquare(new Rect(0, 0, data.filesLinePosX, position.height), setting.fileColor);

            Rect area = new Rect();
            area.position = new Vector2(setting.fileLeftSpace, setting.fileTopSpace);
            area.size = new Vector2(
                data.filesLinePosX - setting.fileRightSpace - setting.fileLeftSpace,
                position.height - setting.fileTopSpace - setting.fileBottomSpace
            );

            GUILayout.BeginArea(area);

            Rect stageFolderPathFieldRect = new Rect();
            stageFolderPathFieldRect.position = area.position;
            stageFolderPathFieldRect.size = new Vector2(area.width - setting.buttonWidth, EditorGUIUtility.singleLineHeight);
            EditorGUIUtility.labelWidth = 100;
            openStageFolderPath = EditorGUI.TextField(stageFolderPathFieldRect, "Stage Name", openStageFolderPath);

            Rect refreshButtonRect = new Rect();
            refreshButtonRect.position = stageFolderPathFieldRect.position + new Vector2(stageFolderPathFieldRect.width, 0);
            refreshButtonRect.size = new Vector2(setting.buttonWidth, EditorGUIUtility.singleLineHeight);

            if (GUI.Button(refreshButtonRect, "Refresh"))
            {
                string folderResourcePath = $"Stage/{openStageFolderPath}";
                if (openStageFolderPath != "" && Directory.Exists($"Assets/Resources/{folderResourcePath}"))
                {
                    data.editorEnemyDataList = Resources.LoadAll<EnemySpawnData>(folderResourcePath + "/EnemySpawnData").ToList();
                }
            }

            if (data.editorEnemyDataList != null)
            {
                for (int i = 0; i < data.editorEnemyDataList.Count; i++)
                {
                    EnemySpawnData spawnData = data.editorEnemyDataList[i];
                    Rect dataObjectRect = new Rect();
                    dataObjectRect.position = area.position + new Vector2(0, i * EditorGUIUtility.singleLineHeight + 2 * EditorGUIUtility.singleLineHeight);
                    dataObjectRect.size = new Vector2(area.width - setting.buttonWidth, EditorGUIUtility.singleLineHeight);
                    EditorGUI.ObjectField(dataObjectRect, spawnData, typeof(EnemyData), false);

                    Rect dataSelectButtonRect = new Rect();
                    dataSelectButtonRect.position = dataObjectRect.position + new Vector2(dataObjectRect.width, 0);
                    dataSelectButtonRect.size = new Vector2(setting.buttonWidth, EditorGUIUtility.singleLineHeight);
                    if (GUI.Button(dataSelectButtonRect, "Select"))
                    {
                        data.selectedEnemySpawnData = spawnData;
                        data.selectedEnemyIndex = i;
                    }
                }
            }
            if (data.selectedEnemySpawnData == null) data.selectedEnemyIndex = -1;
            else
            {
                Rect selectBoxRect = new Rect();
                selectBoxRect.position = area.position + new Vector2(0, data.selectedEnemyIndex * EditorGUIUtility.singleLineHeight + 2 * EditorGUIUtility.singleLineHeight);
                selectBoxRect.size = new Vector2(area.width, EditorGUIUtility.singleLineHeight);
                CustomGUI.DrawSquare(selectBoxRect, setting.selectBoxColor);
            }

            GUILayout.EndArea();
        }
        void DrawInspectorGUI()
        {
            Rect area = new Rect();
            area.position = inspectorRect.position + new Vector2(setting.inspectorLeftSpace, setting.inspectorTopSpace);
            area.size = inspectorRect.size - new Vector2(setting.inspectorRightSpace + setting.inspectorLeftSpace, 0);

            CustomGUI.DrawSquare(inspectorRect, setting.inspectorColor);

            GUILayout.BeginArea(area);

            GUILayout.Label("Enemy Inspector", EditorStyles.boldLabel);

            if (data.selectedEnemySpawnData != null)
            {
                CustmGUILayout.InteractionObjectField(data.selectedEnemySpawnData.enemyPrefab, Header);

                void Header(Rect rect)
                {
                    dropDownRect = new Rect(rect.position.GetSetY(5 + EditorStyles.boldLabel.lineHeight + rect.height), new Vector2(area.width, 23));

                    if (e.isMouse)
                    {
                        if (EventUtility.MouseDown(0) && rect.Contains(e.mousePosition))
                        {
                            isPrefabDropDownExpanded = !isPrefabDropDownExpanded;
                            e.Use();
                        }
                    }
                    if (isPrefabDropDownExpanded && ((dropDownRect.GetAddPositionY(-rect.height).GetAddHeight(23 * (data.prefabs.Length - 1))).Contains(e.mousePosition) == false))
                    {
                        isPrefabDropDownExpanded = false;
                        Debug.Log($"벗어남: {e.mousePosition} // {dropDownRect.GetAddPositionY(-rect.height).GetAddHeight(23 * data.prefabs.Length)}");
                    }
                    
                    if (isPrefabDropDownExpanded)
                    {
                        dropDownIndex = 0;
                        for (int i = 0; i < data.prefabs.Length; i++)
                        {
                            if (dropDownRect.GetAddPositionY(i * 23).Contains(e.mousePosition))
                            {
                                if (EventUtility.MouseDown(0))
                                {
                                    data.selectedEnemySpawnData.enemyPrefab = data.prefabs[dropDownIndex];
                                    isPrefabDropDownExpanded = false;
                                }
                                if (e.isMouse) e.Use();
                            }

                            drawDropDown += () =>
                            {
                                Rect elementRect = dropDownRect.GetAddPositionY(dropDownIndex * 23);
                                CustomGUI.DrawSquare(elementRect, setting.dropDownColor);
                                EditorGUI.LabelField(elementRect, data.prefabs[dropDownIndex].name);
                                if (elementRect.Contains(e.mousePosition)) CustomGUI.DrawSquare(elementRect, setting.selectBoxColor);
                                dropDownIndex++;
                            };
                        }
                    }
                }
                data.selectedEnemySpawnData.spawnTime = EditorGUILayout.FloatField("Spawn Time", data.selectedEnemySpawnData.spawnTime);

                data.selectedEnemyEditorGUI.DrawInspectorGUI(data.selectedEnemySpawnData);

                if (drawDropDown != null)
                {
                    drawDropDown.Invoke();
                    drawDropDown = null;
                }
            }


            GUILayout.EndArea();
        }

        #endregion

    }
    public static Vector2 WorldToScreenPos(Vector2 worldPos)
    {
        return data.previewPos + new Vector2(worldPos.x, -worldPos.y) * data.cellSize;
    }
    public static EnemyEditorGUI GetEnemyEditor()
    {
        EnemyEditorGUI editorInstance;

        Type editorType = data.selectedEnemySpawnData.EditorType;

        if (enemyEditorGUIDictionary.TryGetValue(editorType, out editorInstance) == false)
        {
            editorInstance = (EnemyEditorGUI)Activator.CreateInstance(editorType);
            enemyEditorGUIDictionary.Add(editorType, editorInstance);
        }
        return editorInstance;
    }
}
