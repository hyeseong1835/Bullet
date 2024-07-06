using System;
using System.Collections;
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
    
    public bool debug = false;
    
    enum HoldType
    {
        None,
        Preview,
        FileViewerLine,
        InspectorLine
    }
    HoldType hold = HoldType.None;

    Vector2 offset;
    float prevScreenWidth;

    [MenuItem("Window/StageEditor")]
    static void CreateWindow()
    {
        StageEditor window = (StageEditor)GetWindow(typeof(StageEditor));

        window.Show();
    }
    void OnGUI()
    {
        #region Init

        wantsMouseEnterLeaveWindow = true;
        
        if (data == null) data = (StageEditorData)EditorResources.Load<ScriptableObject>("StageEditor/Data.asset");
        if (setting == null) setting = (StageEditorSetting)EditorResources.Load<ScriptableObject>("StageEditor/Setting.asset");

        #endregion

        RefreshPreviewRect();
        DrawPreview();

        Input();
        
        RefreshFileViewerRect();
        RefreshInspectorRect();

        DrawFileViewerGUI();
        DrawInspectorGUI();

        if (position.width != prevScreenWidth)
        {
            OnScreanWidthResized();
            prevScreenWidth = position.width;
        }

        void OnScreanWidthResized()
        {
            if (data.fileViewerLinePosX < 0)
            {
                data.fileViewerLinePosX = 0;

                Repaint();
            }

            if (data.inspectorLinePosX < 0)
            {
                data.inspectorLinePosX = 0;
                Repaint();
            }
            bool fileViewerLineCorrect = FileViewerWindowCheck(out float fileViewerLineMax);
            bool inspectorLineCorrect = InspectorWindowCheck(out float inspectorLineMax);
            
            if (fileViewerLineCorrect == false && inspectorLineCorrect == false)
            {
                float center = 0.5f * (data.fileViewerLinePosX + (position.width - data.inspectorLinePosX));
                data.fileViewerLinePosX = center - 0.5f * setting.minDistanceBetweenLines;
                data.inspectorLinePosX = center + 0.5f * setting.minDistanceBetweenLines;
            }
            else if (fileViewerLineCorrect == false)
            {
                data.fileViewerLinePosX = fileViewerLineMax;
            }
            else if (inspectorLineCorrect == false)
            {
                data.inspectorLinePosX = inspectorLineMax;
            }
            PreviewCheck();
        }
        
        #region Function

        void Input()
        {
            if (e.isScrollWheel)
            {
                if(previewRect.Contains(e.mousePosition))
                {
                    switch (e.pointerType)
                    {
                        case PointerType.Mouse:
                            data.cellSize += e.delta.y;
                            break;
                    }
                    if (data.cellSize < setting.cellSizeMin) data.cellSize = setting.cellSizeMin;
                    Repaint();
                }
            }
            switch (e.type)
            {
                case EventType.MouseLeaveWindow:
                    switch (hold)
                    {
                        case HoldType.FileViewerLine:
                            if (e.mousePosition.x <= 0)
                            {
                                if (data.fileViewerLinePosX != 0)
                                {
                                    data.fileViewerLinePosX = 0;
                                    Repaint();
                                }
                            }
                            hold = HoldType.None;
                            break;
                        
                        case HoldType.InspectorLine:
                            if (e.mousePosition.x >= position.width)
                            {
                                if (data.inspectorLinePosX != 0)
                                {
                                    data.inspectorLinePosX = 0;
                                    Repaint();
                                }
                            }
                            hold = HoldType.None;
                            break;

                        case HoldType.Preview:
                            hold = HoldType.None;
                            break;
                    }
                    break;
                case EventType.MouseDown:
                    switch (e.button)
                    {
                        case 0: Mouse0Down(); break;
                    }
                    break;

                case EventType.MouseDrag:
                    switch (e.button)
                    {
                        case 0: Mouse0Drag(); break;
                    }
                    break;
                
                case EventType.MouseUp:
                    switch (e.button)
                    {
                        case 0: Mouse0Up(); break;
                    }
                    break;

                case EventType.KeyDown:
                    switch (e.keyCode)
                    {
                        case KeyCode.LeftArrow:
                            if (0 < data.selectedEnemySpawnDataIndex)
                            {
                                data.SelectEnemySpawnData(data.selectedEnemySpawnDataIndex - 1);
                                Repaint();
                            }
                            break;

                        case KeyCode.RightArrow:
                            if (data.selectedEnemySpawnDataIndex != -1 && data.selectedEnemySpawnDataIndex < data.enemySpawnDataList.Count - 1)
                            {
                                data.SelectEnemySpawnData(data.selectedEnemySpawnDataIndex + 1);
                                Repaint();
                            }
                            break;
                        case KeyCode.Comma:
                            if (data.selectedEnemySpawnData != null && 0 < data.selectedEnemySpawnData.spawnTime)
                            {
                                data.selectedEnemySpawnData.spawnTime -= data.timeMoveSnap;
                                data.selectedEnemySpawnData.spawnTime = Mathf.Ceil(data.selectedEnemySpawnData.spawnTime / data.timeMoveSnap) * data.timeMoveSnap;
                                
                                data.SelectEnemySpawnData(data.SortTargetInEnemySpawnDataList(data.selectedEnemySpawnData));
                                Repaint();
                            }
                            break;
                        case KeyCode.Period:
                            if (data.selectedEnemySpawnData != null)
                            {
                                data.selectedEnemySpawnData.spawnTime += data.timeMoveSnap;
                                data.selectedEnemySpawnData.spawnTime = Mathf.Floor(data.selectedEnemySpawnData.spawnTime / data.timeMoveSnap) * data.timeMoveSnap;
                                if (data.selectedEnemySpawnData.spawnTime > data.timeLength)
                                {
                                    data.timeLength = data.selectedEnemySpawnData.spawnTime;
                                }
                                
                                data.SelectEnemySpawnData(data.SortTargetInEnemySpawnDataList(data.selectedEnemySpawnData));
                                Repaint();
                            }
                            break;
                    }
                    break;
            }
            void Mouse0Down()
            {
                if (hold == HoldType.None)
                {
                    float filesLineDistance = e.mousePosition.x - data.fileViewerLinePosX;
                    if (Mathf.Abs(filesLineDistance) < setting.lineHoldWidth)
                    {
                        hold = HoldType.FileViewerLine;
                        
                        GUI.FocusControl(null);
                        e.Use();
                        return;
                    }

                    float inspectorLineDistance = e.mousePosition.x - (position.width - data.inspectorLinePosX);
                    if (Mathf.Abs(inspectorLineDistance) < setting.lineHoldWidth)
                    {
                        hold = HoldType.InspectorLine;
                        
                        GUI.FocusControl(null);
                        e.Use();
                        return;
                    }

                    if (previewRect.Contains(e.mousePosition))
                    {
                        hold = HoldType.Preview;
                        offset = data.previewPos - e.mousePosition;
                        
                        GUI.FocusControl(null);
                        e.Use();
                        return;
                    }
                }
            }
            void Mouse0Drag()
            {
                switch (hold)
                {
                    case HoldType.None: 
                        
                        break;

                    case HoldType.FileViewerLine:
                        data.fileViewerLinePosX = e.mousePosition.x;
                        PreviewCheck();
                        
                        if (InspectorWindowCheck(out float inspectorLineMax) == false)
                        {
                            data.inspectorLinePosX = inspectorLineMax;
                        }
                        
                        Repaint();
                        break;

                    case HoldType.InspectorLine:
                        data.inspectorLinePosX = position.width - e.mousePosition.x;
                        PreviewCheck();
                        
                        if (FileViewerWindowCheck(out float fileViewerLineMax) == false)
                        {
                            data.fileViewerLinePosX = fileViewerLineMax;
                        }
                        
                        Repaint();
                        break;

                    case HoldType.Preview:
                        data.previewPos = e.mousePosition + offset;
                        
                        PreviewCheck();

                        Repaint();
                        break;
                }
            }
            void Mouse0Up()
            {
                if (hold != HoldType.None)
                {
                    hold = HoldType.None;
                }
            }
        }

        void PreviewCheck()
        {
            if (data.preview.IsExit(data.previewPos, position.height, 0, (position.width - data.inspectorLinePosX), data.fileViewerLinePosX, out Vector2 previewContact))
            {
                data.previewPos = previewContact;
            }
        }
        void RefreshFileViewerRect()
        {
            fileViewerRect.position = new Vector2(0, 0);
            fileViewerRect.size = new Vector2(data.fileViewerLinePosX, position.height);
        }
        void RefreshInspectorRect()
        {
            inspectorRect.position = new Vector2(position.width - data.inspectorLinePosX, 0);
            inspectorRect.size = new Vector2(data.inspectorLinePosX, position.height);
        }
        void RefreshPreviewRect()
        {
            previewRect.position = new Vector2(data.fileViewerLinePosX, 0);
            previewRect.size = new Vector2(position.width - (data.inspectorLinePosX + data.fileViewerLinePosX), position.height);
        }

        void DrawFileViewerGUI()
        {
            CustomGUI.DrawSquare(new Rect(0, 0, data.fileViewerLinePosX, position.height), setting.fileColor);

            Rect area = new Rect();
            area.position = new Vector2(setting.fileLeftSpace, setting.fileTopSpace);
            area.size = new Vector2(
                data.fileViewerLinePosX - setting.fileRightSpace - setting.fileLeftSpace,
                position.height - setting.fileTopSpace - setting.fileBottomSpace
            );
            GUILayout.BeginArea(area);
            data.enemyScroll = EditorGUILayout.BeginScrollView(new Vector2(0, data.enemyScroll)).y;
            {
                if (EditorGUILayout.DropdownButton(new GUIContent("Debug"), FocusType.Passive))
                {
                    debug = !debug;
                    Repaint();
                }
                if(debug)
                {
                    EditorGUILayout.ObjectField(data, typeof(StageEditorData), false);
                    EditorGUILayout.ObjectField(setting, typeof(StageEditorSetting), false);

                    TitleHeaderLabel("Stage");
                    EditorGUILayout.ObjectField(data.selectedStage, typeof(Stage), false);
                    {
                        BeginNewTab();
                        {
                            TitleHeaderLabel("Stage List");
                            if (data.stageArray == null) WarningLabel("Stage Array is null");
                            else if (data.stageArray.Length < 1) WarningLabel("Stage Array is Empty");
                            else
                            {
                                foreach (Stage stage in data.stageArray)
                                {
                                    EditorGUILayout.ObjectField(stage, typeof(Stage), false);
                                }
                            }
                        }
                        EndNewTab();
                    }
                    TitleHeaderLabel("Select");
                    EditorGUILayout.ObjectField(data.selectedEnemySpawnData, typeof(EnemySpawnData), false);
                    /*
                    string selectedEnemyEditorGUIText;
                    if (data.selectedEnemyEditorGUI == null) selectedEnemyEditorGUIText = "NULL";
                    else selectedEnemyEditorGUIText = data.selectedEnemyEditorGUI.GetType().Name;
                    EditorGUILayout.TextField(selectedEnemyEditorGUIText);
                    */
                    BeginNewTab();
                    {
                        TitleHeaderLabel("Prefab");
                        EditorGUILayout.ObjectField(data.selectedPrefab, typeof(GameObject), false);
                        if (data.prefabLists == null || data.prefabLists.Count == 0)
                        {
                            WarningLabel("Empty PrefabList");
                        }
                        else
                        {
                            BeginNewTab();
                            {
                                for (int listIndex = 0; listIndex < data.prefabLists.Count; listIndex++)
                                {
                                    List<GameObject> curList = data.prefabLists[listIndex];
                                    Type listType = data.prefabTypeList[listIndex];

                                    if (curList.Count < 1)
                                    {
                                        WarningLabel("Empty List");
                                        continue;
                                    }
                                    CustomGUILayout.UnderBarTitleText(listType.Name);

                                    for (int elementIndex = 0; elementIndex < curList.Count; elementIndex++)
                                    {
                                        EditorGUILayout.ObjectField(curList[elementIndex], typeof(GameObject), false);
                                    }
                                }
                            }
                            EndNewTab();
                        }
                    }
                    EndNewTab();
                }

                GUILayout.Label("File Viewer", EditorStyles.boldLabel);
                
                //Stage Select
                GUILayout.BeginHorizontal();
                {
                    if (data.stageArray == null)
                    {
                        WarningLabel("StageArray is null");
                    }
                    else
                    {
                        string[] stageNameArray = new string[data.stageArray.Length + 1];
                        stageNameArray[0] = "None";
                        for (int i = 0; i < data.stageArray.Length; i++)
                        {
                            Stage stage = data.stageArray[i];
                            if (stage == null) stageNameArray[i] = "NULL";
                            else stageNameArray[i + 1] = stage.name;
                        }

                        int stageSelectInput = EditorGUILayout.Popup(
                            data.selectedStageIndex + 1,
                            stageNameArray,
                            GUILayout.Height(setting.buttonHeight)
                        ) - 1;

                        if (stageSelectInput != data.selectedStageIndex)
                        {
                            data.SelectStage(stageSelectInput);
                        }
                    }
                    
                    //Refresh
                    if (GUI.Button(GUILayoutUtility.GetRect(setting.buttonWidth, setting.buttonHeight).GetAddY(2), "Refresh"))
                    {
                        data.RefreshStageArray();
                        data.RefreshTimeFoldout();
                    }
                }
                GUILayout.EndHorizontal();
                
                EditorGUILayout.Space(5);

                EditorGUILayout.LabelField("Spawn Data", EditorStyles.boldLabel);
                if (data.enemySpawnDataList == null || data.enemySpawnDataList.Count < 1)
                {
                    WarningLabel("EnemySpawnData is Empty");
                }
                else
                {
                    // Enemy List
                    Rect selectRect = Rect.zero;
                    Rect headerRect = Rect.zero;
                    bool isSelectHideByHeader = false;
                    int elementCount = 0;
                    float prevTime = -1;
                    bool foldout = false;
                    for (int i = 0; i < data.enemySpawnDataList.Count; i++)
                    {
                        EnemySpawnData spawnData = data.enemySpawnDataList[i];
                        if (spawnData.spawnTime != prevTime)
                        {
                            if (foldout == false) LateCloseHeader();

                            if (data.timeFoldout.TryGetValue(spawnData.spawnTime, out foldout) == false)
                            {
                                data.timeFoldout.Add(spawnData.spawnTime, true);
                                foldout = true;
                            }

                            if (foldout)
                            {
                                OpenHeader(spawnData, i);
                            }
                            else
                            {
                                CloseHeader(spawnData, i);
                            }
                            elementCount = 0;
                        }
                        else
                        {
                            elementCount++;

                            if (foldout)
                            {
                                OpenElement(spawnData, i);
                            }
                            else
                            {
                                CloseElement(spawnData, i);
                            }
                        }

                        prevTime = spawnData.spawnTime;
                    }
                    if (selectRect != Rect.zero)
                    {
                        if (isSelectHideByHeader)
                        {
                            HidedSelect();
                        }
                        else Select();
                    }
                    if (foldout == false) LateCloseHeader();

                    #region Function

                    void HidedSelect()
                    {
                        Vector2 p1 = headerRect.position.GetAddY(headerRect.height);
                        Vector2 p2 = p1.GetAddX(headerRect.width);
                        Handles.color = setting.selectHideInHeaderColor;
                        Handles.DrawLine(p1, p2);
                    }
                    void Select()
                    {
                        CustomGUI.DrawSquare(selectRect, setting.selectBoxColor);
                    }
                    void LateCloseHeader()
                    {
                        if (elementCount > 0)
                        {
                            Vector2 p1 = headerRect.position.GetAddY(headerRect.height);
                            Vector2 p2 = p1.GetAddX(headerRect.width);
                            Handles.color = Color.white;
                            Handles.DrawLine(p1, p2);

                            EditorGUILayout.Space(5);
                        }
                    }
                    void OpenHeader(EnemySpawnData spawnData, int i)
                    {
                        EditorGUILayout.Space(5);

                        GUILayout.BeginHorizontal();
                        {
                            //Time Label
                            Rect timeRect = GUILayoutUtility.GetRect(setting.timeWidth, EditorGUIUtility.singleLineHeight);
                            EditorGUI.LabelField(timeRect, spawnData.spawnTime.ToString("F1"));

                            if (EventUtility.MouseDown(0) && timeRect.Contains(e.mousePosition))
                            {
                                data.timeFoldout[spawnData.spawnTime] = !foldout;
                                Repaint();
                            }

                            //Object Field
                            EditorGUILayout.ObjectField(spawnData, typeof(EnemyData), false, GUILayout.Height(setting.buttonHeight));

                            headerRect = GUILayoutUtility.GetLastRect().GetSetWidth(area.width - timeRect.width);
                            if (spawnData == data.selectedEnemySpawnData)
                            {
                                selectRect = headerRect;
                            }

                            //Select Button
                            if (GUI.Button(GUILayoutUtility.GetRect(setting.buttonWidth, setting.buttonHeight).GetAddY(2), "Select"))
                            {
                                GUI.FocusControl(null);
                                data.SelectEnemySpawnData(i);
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    void CloseHeader(EnemySpawnData spawnData, int i)
                    {
                        OpenHeader(spawnData, i);
                    }

                    void OpenElement(EnemySpawnData spawnData, int i)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            Rect timeRect = GUILayoutUtility.GetRect(setting.timeWidth, EditorGUIUtility.singleLineHeight);

                            EditorGUILayout.ObjectField(spawnData, typeof(EnemyData), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                            if (spawnData == data.selectedEnemySpawnData)
                            {
                                selectRect = GUILayoutUtility.GetLastRect().GetSetWidth(area.width - timeRect.width);
                            }

                            if (GUI.Button(GUILayoutUtility.GetRect(setting.buttonWidth, setting.buttonHeight).GetAddY(2), "Select"))
                            {
                                GUI.FocusControl(null);
                                data.SelectEnemySpawnData(i);
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    void CloseElement(EnemySpawnData spawnData, int i)
                    {
                        if (spawnData == data.selectedEnemySpawnData)
                        {
                            selectRect = headerRect;
                            isSelectHideByHeader = true;
                        }
                    }

                    #endregion
                }
            }
            EditorGUILayout.Space(100);
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();


            Rect rect = new Rect();
            rect.size = new Vector2(setting.buttonWidth, setting.buttonHeight);
            rect.position = area.position + area.size - rect.size;

            if (GUI.Button(rect, "Apply"))
            {
                data.ApplyToStage();
                EditorUtility.SetDirty(data);
                EditorUtility.SetDirty(setting);
            }

        }
        void DrawInspectorGUI()
        {
            Rect area = new Rect();
            area.position = inspectorRect.position + new Vector2(setting.inspectorLeftSpace, setting.inspectorTopSpace);
            area.size = inspectorRect.size - new Vector2(setting.inspectorRightSpace + setting.inspectorLeftSpace, 0);

            CustomGUI.DrawSquare(inspectorRect, setting.inspectorColor);

            GUILayout.BeginArea(area);
            {
                GUILayout.Label("Enemy Inspector", EditorStyles.boldLabel);

                if (data.selectedEnemySpawnData != null)
                {
                    #region SpawnPrefab

                    List<GameObject> prefabList = data.GetPrefabList(data.selectedEnemySpawnData);

                    string[] selectablePrefabNameArray;
                    if (prefabList == null)
                    {
                        WarningLabel("Cannot Found Type: " + data.selectedEnemySpawnData.GetType());
                    }
                    else
                    {
                        selectablePrefabNameArray = prefabList.Select(prefab => prefab.name).ToArray();

                        int prefabIndexInput = EditorGUILayout.Popup(
                                data.selectedEnemySpawnData.prefabIndex, selectablePrefabNameArray
                            );
                        if (prefabIndexInput != data.selectedEnemySpawnData.prefabIndex)
                        {
                            data.SelectPrefab(prefabIndexInput);
                        }
                    }
                    #endregion
               
                    #region SpawnTime
                    float spawnTimeInput = EditorGUILayout.FloatField("Spawn Time", data.selectedEnemySpawnData.spawnTime);
                    if (spawnTimeInput != data.selectedEnemySpawnData.spawnTime)
                    {
                        if (spawnTimeInput >= 0) data.selectedEnemySpawnData.spawnTime = spawnTimeInput;
                        else data.selectedEnemySpawnData.spawnTime = 0;

                        data.SelectEnemySpawnData(data.SortTargetInEnemySpawnDataList(data.selectedEnemySpawnData));
                    }
                    #endregion

                    if (data.selectedEnemyEditorGUI == null) data.RefreshEnemyEditorGUI();

                    if (data.selectedEnemyEditorGUI != null) data.selectedEnemyEditorGUI.DrawInspectorGUI(data.selectedEnemySpawnData);
                    else WarningLabel("EditorGUI Missing");
                }
                else
                {
                    WarningLabel("Select Enemy Spawn Data");
                }
            }
            GUILayout.EndArea();
        }
        void DrawPreview()
        {
            RefreshPreviewRect();
            CustomGUI.DrawSquare(previewRect, setting.previewBackGroundColor);
            Vector2Int cellCount = 3 * Vector2Int.one + new Vector2Int(
                Mathf.FloorToInt(previewRect.width / data.cellSize),
                Mathf.FloorToInt(previewRect.height / data.cellSize)
            );
            Vector2 offset = new Vector2((data.previewPos.x) % data.cellSize, (data.previewPos.y) % data.cellSize);
            Vector2 start = new Vector2(Mathf.Floor(previewRect.x / data.cellSize) * data.cellSize , Mathf.Floor(previewRect.y / data.cellSize) * data.cellSize);
            CustomGUI.DrawOpenGrid(start + offset - data.cellSize * Vector2.one, cellCount, data.cellSize, setting.previewOutGridColor);
            CustomGUI.DrawCloseGrid(data.previewPos + data.cellSize * new Vector2(-0.5f * Window.GameWidth, -Window.GameHeight), new Vector2Int(Window.GameWidth, Window.GameHeight), data.cellSize, setting.previewGameGridColor);

            if (data.selectedEnemyEditorGUI != null) data.selectedEnemyEditorGUI.DrawEnemyDataGizmos(data.selectedEnemySpawnData);
            
            if (data.selectedEnemySpawnData != null) DrawTimeLine();
        }

        void DrawTimeLine()
        {
            float timeLineStart = data.fileViewerLinePosX + setting.timeHorizontalSpace;
            float timeLineEnd = (position.width - data.inspectorLinePosX) - setting.timeHorizontalSpace;

            if (timeLineStart > timeLineEnd) return;

            float timeLineX = data.fileViewerLinePosX + setting.timeHorizontalSpace;
            float timeLineY = position.height - setting.timeBottomSpace;
            float timeLineWidth = (position.width - data.inspectorLinePosX) - 2 * setting.timeHorizontalSpace;

            Handles.color = setting.timeLineColor;
            Handles.DrawLine(
                new Vector3(timeLineStart, timeLineY),
                new Vector3(timeLineEnd, timeLineY)
            );
            Handles.DrawLine(
                new Vector2(timeLineStart, timeLineY + setting.timeLengthFieldOffsetY),
                new Vector2(timeLineStart, timeLineY - setting.timeLengthFieldOffsetY)
            );
            Handles.DrawLine(
                new Vector2(timeLineEnd, timeLineY + setting.timeLengthFieldOffsetY),
                new Vector2(timeLineEnd, timeLineY - setting.timeLengthFieldOffsetY)
            );
            for (int i = 0; i < data.enemySpawnDataList.Count; i++)
            {
                DrawTimeLineMarker(
                    data.enemySpawnDataList[i].spawnTime, 
                    setting.enemySpawnTimeColor
                );
            }
            if (0 <= data.selectedEnemySpawnDataIndex
                && data.selectedEnemySpawnDataIndex < data.enemySpawnDataList.Count)
            {
                DrawTimeLineMarker(
                    data.enemySpawnDataList[data.selectedEnemySpawnDataIndex].spawnTime,
                    setting.selectEnemySpawnTimeColor
                );
            }
            
            void DrawTimeLineMarker(float time, SquareColor color)
            {
                float timeRatio = time / data.timeLength;

                Rect timeLineMarkerRect = new Rect();
                timeLineMarkerRect.position = new Vector2(
                    Mathf.Lerp(timeLineStart, timeLineEnd, timeRatio) - 0.5f * setting.timeCubeSize,
                    timeLineY
                ) - Vector2.one * 0.5f * setting.timeCubeSize;
                timeLineMarkerRect.size = Vector2.one * setting.timeCubeSize;

                CustomGUI.DrawSquare(timeLineMarkerRect, color);
            }
            Rect timeLengthFieldRect = new Rect();
            timeLengthFieldRect.position = new Vector2(timeLineEnd, timeLineY - (setting.timeLengthFieldSize.y + setting.timeLengthFieldOffsetY));
            timeLengthFieldRect.size = setting.timeLengthFieldSize;
            
            data.timeLength = EditorGUI.DelayedFloatField(timeLengthFieldRect, data.timeLength);
            if (data.enemySpawnDataList.Count >= 1)
            {
                float lastTime = data.enemySpawnDataList[^1].spawnTime;
                if (lastTime > data.timeLength) data.timeLength = lastTime;
            }
        }

        #endregion
        void WarningLabel(string message)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField(message, EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space(10);
        }
        void TitleHeaderLabel(string title)
        {
            EditorGUILayout.Space(5);
            CustomGUILayout.UnderBarTitleText(title);
        }
        void BeginNewTab()
        {
            float areaWidth = EditorGUILayout.BeginHorizontal().width;
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginVertical(GUILayout.Width(areaWidth));
        }
        void EndNewTab()
        {
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }

    bool FileViewerWindowCheck()
    {
        float fileViewerLineMax = position.width - (data.inspectorLinePosX + setting.minDistanceBetweenLines);

        return data.fileViewerLinePosX <= fileViewerLineMax;
    }
    bool FileViewerWindowCheck(out float fileViewerLineMax)
    {
        fileViewerLineMax = position.width - (data.inspectorLinePosX + setting.minDistanceBetweenLines);

        return data.fileViewerLinePosX <= fileViewerLineMax;
    }
    bool InspectorWindowCheck()
    {
        float inspectorLineMax = position.width - (data.fileViewerLinePosX + setting.minDistanceBetweenLines);
        return data.inspectorLinePosX <= inspectorLineMax;
    }
    bool InspectorWindowCheck(out float inspectorLineMax)
    {
        inspectorLineMax = position.width - (data.fileViewerLinePosX + setting.minDistanceBetweenLines);
        return data.inspectorLinePosX <= inspectorLineMax;
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