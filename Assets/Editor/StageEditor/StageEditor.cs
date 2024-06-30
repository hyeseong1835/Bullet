using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

public class StageEditor : EditorWindow
{
    static StageEditorData data;
    static StageEditorSetting setting;

    bool holdInspectorLine = false;
    bool holdFilesLine = false;

    float timeLength = 100;

    string openStageFolderPath = "";

    [MenuItem("Window/StageEditor")]
    static void CreateWindow()
    {
        StageEditor window = (StageEditor)EditorWindow.GetWindow(typeof(StageEditor));

        window.Show();
    }
    void OnGUI()
    {
        Event e = Event.current;

        LoadData();

        PreviewMove();

        DrawGrid();

        InspectorLineHold();
        FileLineHold();

        ShowEnemyDataOnInspector();
        ShowFilesOnFileViewer();

        DrawEnemyDataGizmos();
        DrawTimeLine();
        



        void LoadData()
        {
            if (data == null) data = (StageEditorData)EditorResources.Load<ScriptableObject>("StageEditor/Data.asset");
            if (setting == null) setting = (StageEditorSetting)EditorResources.Load<ScriptableObject>("StageEditor/Setting.asset");
        }
        void DrawGrid()
        {
            Handles.color = Color.gray;
            for (float x = data.previewPos.x % data.cellSize; x < data.inspectorLinePosX; x += data.cellSize)
            {
                Vector3 p1 = new Vector3(x, 0);
                Vector3 p2 = new Vector3(x, position.height);

                Handles.DrawLine(p1, p2);
            }
            for (float y = data.previewPos.y % data.cellSize; y < position.height; y += data.cellSize)
            {
                Vector3 p1 = new Vector3(0, y);
                Vector3 p2 = new Vector3(data.inspectorLinePosX, y);

                Handles.DrawLine(p1, p2);
            }
            Handles.color = Color.white;

            for (float x = -0.5f * Window.GameWidth * data.cellSize;
                x <= 0.5f * Window.GameWidth * data.cellSize;
                x += data.cellSize)
            {
                Vector3 p1 = data.previewPos;
                p1.x += x;

                Vector3 p2 = data.previewPos;
                p2.x += x;
                p2.y -= data.cellSize * Window.GameHeight;

                Handles.DrawLine(p1, p2);
            }
            for (float y = 0;
                y <= data.cellSize * Window.GameHeight;
                y += data.cellSize)
            {
                float xHalf = 0.5f * data.cellSize * Window.GameWidth;

                Vector3 p1 = data.previewPos;
                p1.x -= xHalf;
                p1.y -= y;

                Vector3 p2 = data.previewPos;
                p2.x += xHalf;
                p2.y -= y;

                Handles.DrawLine(p1, p2);
            }

        }
        void PreviewMove()
        {
            Vector2 move;
            switch (e.keyCode)
            {
                case KeyCode.LeftArrow:
                    move = Vector2.left;
                    break;

                case KeyCode.RightArrow:
                    move = Vector2.right;
                    break;

                case KeyCode.UpArrow:
                    move = Vector2.down;
                    break;

                case KeyCode.DownArrow:
                    move = Vector2.up;
                    break;

                default:
                    move = Vector2.zero;
                    break;
            }
            if (move != Vector2.zero)
            {
                data.previewPos += move * setting.screenMoveSpeed;
                Repaint();
            }
            if (data.preview.IsContact(data.previewPos, position.height, 0, data.inspectorLinePosX, data.filesLinePosX, out Vector2 previewContact))
            {
                data.previewPos = previewContact;
            }
        }

        void InspectorLineHold()
        {
            Handles.DrawSolidRectangleWithOutline(
                new Rect(data.inspectorLinePosX, 0, position.width - data.inspectorLinePosX, position.height),
                new Color(0.1f, 0.1f, 0.1f, 1f),
                Color.black
            );
            switch (e.type)
            {
                case EventType.MouseDown:
                    float distance = e.mousePosition.x - data.inspectorLinePosX;
                    if (Mathf.Abs(distance) < setting.lineHoldWidth)
                    {
                        holdInspectorLine = true;
                        e.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (holdInspectorLine)
                    {
                        data.inspectorLinePosX = e.mousePosition.x;
                        if (data.inspectorLinePosX - data.filesLinePosX < setting.minDistanceBetweenLines)
                        {
                            data.filesLinePosX = data.inspectorLinePosX - setting.minDistanceBetweenLines;
                        }
                        e.Use();
                        Repaint();
                    }
                    break;
                case EventType.MouseUp:
                    if (holdInspectorLine)
                    {
                        holdInspectorLine = false;
                    }
                    break;
            }
            if (data.inspectorLinePosX < 0 || position.width - setting.minDistanceBetweenLines < data.inspectorLinePosX)
            {
                data.inspectorLinePosX = position.width;
                Repaint();
            }
        }
        void FileLineHold()
        {
            Handles.DrawSolidRectangleWithOutline(
                new Rect(0, 0, data.filesLinePosX, position.height),
                new Color(0.1f, 0.1f, 0.1f, 1f),
                Color.black
            );
            switch (e.type)
            {
                case EventType.MouseDown:
                    float distance = e.mousePosition.x - data.filesLinePosX;
                    if (Mathf.Abs(distance) < setting.lineHoldWidth)
                    {
                        holdFilesLine = true;
                        e.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (holdFilesLine)
                    {
                        data.filesLinePosX = e.mousePosition.x;
                        if (data.inspectorLinePosX - data.filesLinePosX < setting.minDistanceBetweenLines)
                        {
                            data.inspectorLinePosX = data.filesLinePosX + setting.minDistanceBetweenLines;
                        }
                        e.Use();
                        OnGUI();
                    }
                    break;
                case EventType.MouseUp:
                    if (holdFilesLine)
                    {
                        holdFilesLine = false;
                    }
                    break;
            }

            if (data.filesLinePosX < 0 || position.width - setting.minDistanceBetweenLines < data.filesLinePosX)
            {
                data.filesLinePosX = 0;
                Repaint();
            }
        }
        
        void ShowEnemyDataOnInspector()
        {
            GUILayout.BeginArea(new Rect(
                data.inspectorLinePosX + setting.inspectorLeftSpace, setting.inspectorTopSpace,
                position.width - data.inspectorLinePosX - setting.inspectorLeftSpace - setting.inspectorRightSpace, position.height));

            GUILayout.Label("Enemy Inspector", EditorStyles.boldLabel);

            Vector2 editedselectedEnemyDataWorldPos = EditorGUILayout.Vector2Field("World Pos", data.selectedEnemy.worldPos);
            if (editedselectedEnemyDataWorldPos != data.selectedEnemy.worldPos)
            {
                data.selectedEnemy.worldPos = editedselectedEnemyDataWorldPos;
            }
            data.selectedEnemy.definition = EditorGUILayout.Vector2Field("Definition", data.selectedEnemy.definition);

            data.selectedEnemy.spawnTime = EditorGUILayout.FloatField("Spawn Time", data.selectedEnemy.spawnTime);

            GUILayout.EndArea();



            Rect applyButtonRect = new Rect();
            applyButtonRect.x = position.width - (setting.buttonWidth + setting.inspectorRightSpace);
            applyButtonRect.y = position.height - 10 - 0.5f * setting.buttonHeight - setting.inspectorBottomSpace;
            applyButtonRect.width = setting.buttonWidth;
            applyButtonRect.height = setting.buttonHeight;

            Rect loadButtonRect = new Rect();
            loadButtonRect.x = data.inspectorLinePosX + (setting.inspectorLeftSpace);
            loadButtonRect.y = position.height - 10 - 0.5f * setting.buttonHeight - setting.inspectorBottomSpace;
            loadButtonRect.width = setting.buttonWidth;
            loadButtonRect.height = setting.buttonHeight;
            if (GUI.Button(applyButtonRect, "Apply"))
            {

            }
            if (GUI.Button(loadButtonRect, "Load"))
            {

            }
        }
        void ShowFilesOnFileViewer()
        {
            Rect area = new Rect();
            area.position = new Vector2(setting.fileLeftSpace, setting.fileTopSpace);
            area.size = new Vector2(data.filesLinePosX - setting.fileRightSpace - setting.fileLeftSpace, position.height - setting.fileTopSpace - setting.fileBottomSpace);

            GUILayout.BeginArea(area);

            Rect stageFolderPathFieldRect = new Rect();
            stageFolderPathFieldRect.position = area.position + new Vector2(0, setting.buttonHeight);
            stageFolderPathFieldRect.size = new Vector2(area.width - setting.buttonWidth, EditorGUIUtility.singleLineHeight);
            openStageFolderPath = EditorGUI.TextField(stageFolderPathFieldRect, "Stage Folder Path", openStageFolderPath);
            
            Rect refreshButtonRect = new Rect();
            refreshButtonRect.position = area.position + new Vector2(stageFolderPathFieldRect.width, setting.buttonHeight);
            refreshButtonRect.size = new Vector2(setting.buttonWidth, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(refreshButtonRect, "Refresh"))
            {
                string folderResourcePath = $"Stage/{openStageFolderPath}";
                if (openStageFolderPath != "" && Directory.Exists($"Assets/Resources/{folderResourcePath}"))
                {
                    // = (EnemyData)Resources.LoadAll(folderResourcePath + "Enemies");
                }
            }

            GUILayout.EndArea();
        }

        void DrawEnemyDataGizmos()
        {
            Vector2 screenPos = WorldToScreenPos(data.selectedEnemy.worldPos);

            Handles.color = Color.cyan;

            Handles.DrawWireDisc(screenPos, Vector3.forward, 10);

            Vector2 definitionScreenPos = WorldToScreenPos(data.selectedEnemy.definition);

            Handles.DrawLine(screenPos, definitionScreenPos);
            Vector2 X = new Vector2(setting.definitionGizmoSize, -setting.definitionGizmoSize);
            Handles.DrawLine(definitionScreenPos + X, definitionScreenPos - X);
            Handles.DrawLine(definitionScreenPos + Vector2.one * setting.definitionGizmoSize, definitionScreenPos - Vector2.one * setting.definitionGizmoSize);

        }
        void DrawTimeLine()
        {
            float timeLineY = position.height - setting.timeBottomSpace;

            Handles.color = Color.white;
            Handles.DrawLine(
                    new Vector3(data.filesLinePosX + setting.timeHorizontalSpace, timeLineY),
                    new Vector3(data.inspectorLinePosX - setting.timeHorizontalSpace, timeLineY)
                );
            Handles.DrawWireCube(
                new Vector3(data.filesLinePosX + setting.timeHorizontalSpace + (data.selectedEnemy.spawnTime / timeLength) * (data.inspectorLinePosX - 2 * setting.timeHorizontalSpace), timeLineY),
                Vector3.one * setting.timeCubeSize
            );
        }
    }
    public static Vector2 WorldToScreenPos(Vector2 worldPos)
    {
        return data.previewPos + new Vector2(worldPos.x, -worldPos.y) * data.cellSize;
    }
}
