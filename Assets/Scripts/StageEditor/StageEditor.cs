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

    enum HoldType
    {
        None,
        Preview,
        FileViewerLine,
        InspectorLine
    }
    HoldType hold = HoldType.None;

    float timeLength = 100;

    Vector2 offset;

    [MenuItem("Window/StageEditor")]
    static void CreateWindow()
    {
        StageEditor window = (StageEditor)EditorWindow.GetWindow(typeof(StageEditor));

        window.Show();
    }
    void OnGUI()
    {
        #region Init

        if (data == null) data = (StageEditorData)EditorResources.Load<ScriptableObject>("StageEditor/Data.asset");
        if (setting == null) setting = (StageEditorSetting)EditorResources.Load<ScriptableObject>("StageEditor/Setting.asset");

        #endregion

        if (e.isScrollWheel)
        {
            data.cellSize += e.delta.y;
            Repaint();
        }
        RefreshPreviewRect();
        DrawPreview();

        #region FileViewer Move

        if (hold == HoldType.None && EventUtility.MouseDown(0))
        {
            float distance = e.mousePosition.x - data.filesLinePosX;
            if (Mathf.Abs(distance) < setting.lineHoldWidth)
            {
                hold = HoldType.FileViewerLine;
                e.Use();
            }
        }
        if (hold == HoldType.FileViewerLine && EventUtility.MouseDrag(0))
        {
            data.filesLinePosX = e.mousePosition.x;

            float inspectorLineMin = data.filesLinePosX + setting.minDistanceBetweenLines;
            if (data.inspectorLinePosX < inspectorLineMin)
            {
                data.inspectorLinePosX = inspectorLineMin;
                RefreshInspectorRect();
            }
            Repaint();
            e.Use();
        }

        if (data.filesLinePosX < 0)
        {
            data.filesLinePosX = 0;

            Repaint();
        }

        #endregion

        #region Inspector Move

        if (hold == HoldType.None && EventUtility.MouseDown(0))
        {
            float distance = e.mousePosition.x - data.inspectorLinePosX;
            if (Mathf.Abs(distance) < setting.lineHoldWidth)
            {
                hold = HoldType.InspectorLine;
                e.Use();
            }
        }
        if (hold == HoldType.InspectorLine && EventUtility.MouseDrag(0))
        {
            data.inspectorLinePosX = e.mousePosition.x;
            float fileLineMax = data.inspectorLinePosX - setting.minDistanceBetweenLines;
            if (data.filesLinePosX > fileLineMax)
            {
                data.filesLinePosX = fileLineMax;
                RefreshFileViewerRect();
            }
            Repaint();
            e.Use();
        }
        if (data.inspectorLinePosX > position.width)
        {
            data.inspectorLinePosX = position.width;
            Repaint();
        }

        #endregion
        
        RefreshFileViewerRect();
        RefreshInspectorRect();

        DrawFileViewerGUI();
        DrawInspectorGUI();

        if (hold == HoldType.None && EventUtility.MouseDown(0) && previewRect.Contains(e.mousePosition))
        {
            hold = HoldType.Preview;
            offset = data.previewPos - e.mousePosition;
            e.Use();
        }
        if (hold == HoldType.Preview && EventUtility.MouseDrag(0))
        {
            data.previewPos = e.mousePosition + offset;
            e.Use();

            Repaint();
        }
        if (EventUtility.MouseUp(0))
        {
            hold = HoldType.None;
        }

        if (data.preview.IsExit(data.previewPos, position.height, 0, data.inspectorLinePosX, data.filesLinePosX, out Vector2 previewContact))
        {
            data.previewPos = previewContact;
        }

        if (data.selectedEnemySpawnData != null)
        {
            DrawTimeLine();
        }

        #region Function

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

            GUILayout.Label("File Viewer", EditorStyles.boldLabel);

            StagePopup();

            EnemyPopup();

            GUILayout.EndArea();
            
            void StagePopup()
            {
                GUILayout.BeginHorizontal();
                int stageSelectInput = EditorGUILayout.Popup(data.selectedStageIndex, data.stageNameArray, GUILayout.Height(setting.buttonHeight));
                if (stageSelectInput != data.selectedStageIndex)
                {
                    data.selectedStageIndex = stageSelectInput;
                    string stageFolderPath = $"Stage/{data.stageNameArray[data.selectedStageIndex]}";
                    data.selectedStage = (Stage)Resources.Load<ScriptableObject>($"{stageFolderPath}/Stage");
                    data.editorEnemyDataList = ((EnemySpawnData[])Resources.LoadAll<ScriptableObject>($"Stage/{data.stageNameArray[data.selectedStageIndex]}/EnemySpawnData")).ToList();
                }

                if (GUI.Button(GUILayoutUtility.GetRect(setting.buttonWidth, setting.buttonHeight).GetAddPositionY(2), "Refresh"))
                {
                    data.stageNameArray = Directory.GetDirectories("Assets/Resources/Stage").Select(path => { string splitPath = path.Split('/')[^1]; return splitPath.Substring(6, splitPath.Length - 6); }).ToArray();
                }
                GUILayout.EndHorizontal();
            }
            void EnemyPopup()
            {
                Rect selectBoxRect = new Rect();

                if (data.editorEnemyDataList != null)
                {
                    for (int i = 0; i < data.editorEnemyDataList.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        EnemySpawnData spawnData = data.editorEnemyDataList[i];
                        EditorGUILayout.ObjectField(spawnData, typeof(EnemyData), false, GUILayout.Height(setting.buttonHeight));
                        
                        if (data.selectedEnemyIndex == i) selectBoxRect = GUILayoutUtility.GetLastRect();

                        if (GUI.Button(GUILayoutUtility.GetRect(setting.buttonWidth, setting.buttonHeight).GetAddPositionY(2), "Select"))
                        {
                            data.selectedEnemySpawnData = spawnData;
                            data.selectedEnemyIndex = i;
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                if (data.selectedEnemySpawnData == null) data.selectedEnemyIndex = -1;
                else CustomGUI.DrawSquare(selectBoxRect, setting.selectBoxColor);
            }
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
                int enemySpawnDataInput = EditorGUILayout.Popup(data.selectedEnemySpawnData.prefabIndex, data.prefabs.Select(prefab => prefab.name).ToArray());
                if(data.selectedEnemySpawnData.prefabIndex != enemySpawnDataInput)
                {
                    data.selectedEnemySpawnData.prefabIndex = enemySpawnDataInput;
                    data.selectedEnemyEditorGUI = GetEnemyEditor(data.selectedEnemySpawnData.EditorType);
                }
                if (data.selectedEnemyEditorGUI != null) data.selectedEnemyEditorGUI.DrawInspectorGUI(data.selectedEnemySpawnData);
            }

            GUILayout.EndArea();
        }
        
        void DrawPreview()
        {
            DrawGrid();

            if (data.selectedEnemyEditorGUI != null) data.selectedEnemyEditorGUI.DrawEnemyDataGizmos(data.selectedEnemySpawnData);
            
            void DrawGrid()
            {
                RefreshPreviewRect();
                CustomGUI.DrawSquare(previewRect, setting.previewBackGroundColor);
                Vector2Int floor = new Vector2Int(
                    Mathf.FloorToInt(previewRect.width / data.cellSize), 
                    Mathf.FloorToInt(previewRect.height / data.cellSize)
                );
                Vector2 offset = previewRect.size - floor;
                CustomGUI.DrawCloseGrid(-offset, floor, data.cellSize, setting.previewOutGridColor);
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
                float timeRatio = data.editorEnemyDataList[i].spawnTime / timeLength;

                Rect timeLineMarkerRect = new Rect();
                timeLineMarkerRect.position = new Vector2(timeLineX + timeRatio * timeLineWidth - 0.5f * setting.timeCubeSize, timeLineY - 0.5f * setting.timeCubeSize);
                timeLineMarkerRect.size = Vector2.one * setting.timeCubeSize;

                if (i == data.selectedEnemyIndex)
                {
                    CustomGUI.DrawSquare(timeLineMarkerRect, setting.selectEnemySpawnTimeColor);
                }
                else CustomGUI.DrawSquare(timeLineMarkerRect, setting.enemySpawnTimeColor);
            }
        }

        #endregion

    }
    public static Vector2 WorldToScreenPos(Vector2 worldPos)
    {
        return data.previewPos + new Vector2(worldPos.x, -worldPos.y) * data.cellSize;
    }
    public static EnemyEditorGUI GetEnemyEditor(Type editorType)
    {
        EnemyEditorGUI editorInstance;

        if (enemyEditorGUIDictionary.TryGetValue(editorType, out editorInstance) == false)
        {
            editorInstance = (EnemyEditorGUI)Activator.CreateInstance(editorType);
            enemyEditorGUIDictionary.Add(editorType, editorInstance);
        }
        return editorInstance;
    }
}
