using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

public class StageEditor : EditorWindow
{
    public static StageEditorData data;
    public static StageEditorSetting setting;

    static Dictionary<Type, EnemyEditorGUI> enemyEditorGUIDictionary = new Dictionary<Type, EnemyEditorGUI>();
    
    bool holdInspectorLine = false;
    bool holdFilesLine = false;

    float timeLength = 100;

    string openStageFolderPath = "";

    bool isPrefabDropDownExpanded = false;
    Vector2 offset;
    public bool mouse0 = false;

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


        DrawGrid();
        if (data.selectedEnemySpawnData != null && data.selectedEnemyEditorGUI != null)
        {
            data.selectedEnemyEditorGUI.DrawEnemyDataGizmos(data.selectedEnemySpawnData);
        }
        InspectorLineHold();
        FileLineHold();

        Rect previewRect = new Rect();
        previewRect.x = data.filesLinePosX;
        previewRect.y = 0;
        previewRect.width = data.inspectorLinePosX - data.filesLinePosX;
        previewRect.height = position.height;

        PreviewMove();

        OnFileViewerGUI();

        if (data.selectedEnemySpawnData != null)
        {
            TimeLineMove();
            DrawTimeLine();

            OnInspectorGUI();
        }

        void OnFileViewerGUI()
        {
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
                Handles.DrawSolidRectangleWithOutline(selectBoxRect, setting.selectBoxFaceColor, setting.selectBoxOutlineColor);
            }

            GUILayout.EndArea();
        }

        void OnInspectorGUI()
        {
            Rect area = new Rect();
            area.position = new Vector2(data.inspectorLinePosX + setting.inspectorLeftSpace, setting.inspectorTopSpace);
            area.size = new Vector2(position.width - data.inspectorLinePosX - setting.inspectorLeftSpace - setting.inspectorRightSpace, position.height);

            GUILayout.BeginArea(area);

            GUILayout.Label("Enemy Inspector", EditorStyles.boldLabel);

            data.selectedEnemySpawnData.spawnTime = EditorGUILayout.FloatField("Spawn Time", data.selectedEnemySpawnData.spawnTime);

            Rect prefabDropDownButtonRect = PrefabSelectDropDownHeader();
            
            if (data.selectedEnemyEditorGUI != null)
            {
                EnemySpawnData();
            }
            
            PrefabSelectDropDown(prefabDropDownButtonRect);

            GUILayout.EndArea();

            void EnemySpawnData() 
            {
                data.selectedEnemySpawnData = (EnemySpawnData)EditorGUILayout.ObjectField(data.selectedEnemySpawnData, typeof(EnemySpawnData), false);
                data.selectedEnemyEditorGUI.DrawInspectorGUI(data.selectedEnemySpawnData);
            }
            Rect PrefabSelectDropDownHeader()
            {
                string selectedName;
                if (data.selectedEnemySpawnData.enemyPrefab != null) selectedName = data.selectedEnemySpawnData.enemyPrefab.name;
                else selectedName = "None";

                if (EditorGUILayout.DropdownButton(new GUIContent(selectedName), FocusType.Passive))
                {
                    if (isPrefabDropDownExpanded)
                    {
                        isPrefabDropDownExpanded = false;
                    }
                    else
                    {
                        data.prefabs = Resources.LoadAll<GameObject>($"Stage/{openStageFolderPath}/EnemyPrefabs");
                        isPrefabDropDownExpanded = true;
                    }
                }
                return GUILayoutUtility.GetLastRect();
            }
            void PrefabSelectDropDown(Rect headerRect)
            {
                if (isPrefabDropDownExpanded)
                {
                    for (int i = 0; i < data.prefabs.Length; i++)
                    {
                        GameObject prefab = data.prefabs[i];
                        Rect rect = headerRect;
                        rect.position = rect.position.GetAddY((i + 1) * headerRect.height);
                        GUI.Box(rect, prefab.name);

                        if (rect.Contains(e.mousePosition))
                        {
                            Handles.DrawSolidRectangleWithOutline(rect, new Color(1, 1, 1, 0.1f), Color.white);
                            if (e.isMouse)
                            {
                                if (e.type == EventType.MouseDown)
                                {
                                    data.selectedEnemySpawnData.enemyPrefab = prefab;
                                    isPrefabDropDownExpanded = false;
                                }
                                e.Use();
                            }
                        }
                        Repaint();
                    }

                    //if (dropDownRect.Contains(e.mousePosition) == false){ Debug.Log("닫음 : 범위 벗어남"); isPrefabDropDownExpanded = false; }
                }
            }
        }

        void LoadData()
        {
            if (data == null) data = (StageEditorData)EditorResources.Load<ScriptableObject>("StageEditor/Data.asset");
            if (setting == null) setting = (StageEditorSetting)EditorResources.Load<ScriptableObject>("StageEditor/Setting.asset");

            if (data.selectedEnemySpawnData != null && data.selectedEnemySpawnData.enemyPrefab != null && data.selectedEnemySpawnData != null)
            {
                data.selectedEnemyEditorGUI = GetEnemyEditor();
            }
        }
        void DrawGrid()
        {
            Handles.DrawSolidDisc(WorldToScreenPos(Vector2.zero), Vector3.forward, 0.1f * data.cellSize);
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
            if (e.type == EventType.MouseDown && previewRect.Contains(e.mousePosition))
            {
                offset = data.previewPos - e.mousePosition;
                mouse0 = true;
            }
            if (e.type == EventType.MouseUp)
            {
                mouse0 = false;
            }
            if (mouse0)
            {
                data.previewPos = e.mousePosition + offset;
                Repaint();

                if (data.preview.IsContact(data.previewPos, position.height, 0, data.inspectorLinePosX, data.filesLinePosX, out Vector2 previewContact))
                {
                    data.previewPos = previewContact;
                }
            }
            /*
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
            */
        }
        void TimeLineMove()
        {

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
        
        void DrawTimeLine()
        {
            float timeLineY = position.height - setting.timeBottomSpace;

            Handles.color = Color.white;
            Handles.DrawLine(
                    new Vector3(data.filesLinePosX + setting.timeHorizontalSpace, timeLineY),
                    new Vector3(data.inspectorLinePosX - setting.timeHorizontalSpace, timeLineY)
                );
            for (int i = 0; i < data.editorEnemyDataList.Count; i++)
            {
                if (i == data.selectedEnemyIndex)
                {
                    Handles.DrawSolidRectangleWithOutline(
                    new Rect(
                        data.filesLinePosX + setting.timeHorizontalSpace + (data.selectedEnemySpawnData.spawnTime / timeLength) * (data.inspectorLinePosX - 2 * setting.timeHorizontalSpace) - 0.5f * setting.timeCubeSize,
                        timeLineY - 0.5f * setting.timeCubeSize,
                        setting.timeCubeSize,
                        setting.timeCubeSize
                    ),
                    setting.selectEnemySpawnTimeFaceColor,
                    setting.selectEnemySpawnTimeOutlineColor
                    );
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
