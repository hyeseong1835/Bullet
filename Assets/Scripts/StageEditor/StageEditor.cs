using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

public class StageEditor : EditorWindow
{
    public static StageEditor instance;
    public static StageEditorData data;
    public static StageEditorSetting setting;
    
    static Event e => Event.current;
    
    public Rect fileViewerRect = new Rect();
    public Rect inspectorRect = new Rect();
    public Rect previewRect = new Rect();
    
    public bool debug = false;
    public float playTime = -1;

    PreviewRenderUtility previewRender;
    Texture2D prefabPreview;
    
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

    #region Event

    [MenuItem("Window/StageEditor")]
    public static void CreateWindow()
    {
        instance = (StageEditor)GetWindow(typeof(StageEditor));

        instance.Show();
    }
    void OnValidate()
    {
        instance = this;
    }
    void OnEnable()
    {
        data = (StageEditorData)EditorResources.Load<ScriptableObject>("StageEditor/Data.asset");
        setting = (StageEditorSetting)EditorResources.Load<ScriptableObject>("StageEditor/Setting.asset");

        previewRender = new PreviewRenderUtility();
        Camera gameCam = CameraController.instance.cam;
        previewRender.camera.orthographic = gameCam.orthographic;
        previewRender.camera.orthographicSize = gameCam.orthographicSize;
        previewRender.camera.transform.position = Vector3.zero; //ScreenToWorldPoint(data.previewPos.GetAddY(0.5f * previewRect.size.y));
        previewRender.camera.transform.rotation = gameCam.transform.rotation;
        previewRender.camera.clearFlags = gameCam.clearFlags;
        previewRender.camera.backgroundColor = setting.previewBackGroundColor;
        previewRender.camera.cullingMask = gameCam.cullingMask;
        previewRender.camera.fieldOfView = gameCam.fieldOfView;
        previewRender.camera.nearClipPlane = gameCam.nearClipPlane;
        previewRender.camera.farClipPlane = gameCam.farClipPlane;
    }
    void OnDisable()
    {
        previewRender.camera.targetTexture = null;
        previewRender.Cleanup();
    }
    
    #endregion

    void OnGUI()
    {
        #region Init

        wantsMouseEnterLeaveWindow = true;
        Handles.color = Color.white;
        
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {

        }
        else
        {
            playTime = -1;
        }
#endif

        #endregion

        RefreshPreviewRect();
        DrawPreview();

        Input();
        
        RefreshFileViewerRect();
        RefreshInspectorRect();

        if (data.fileViewerLinePosX != 0) DrawFileViewerGUI();
        if (data.inspectorLinePosX != 0) DrawInspectorGUI();

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

        #region GUI

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
            {
                data.enemyScroll = EditorGUILayout.BeginScrollView(new Vector2(0, data.enemyScroll), false, true, GUIStyle.none, GUI.skin.verticalScrollbar, GUIStyle.none).y;
                area.width -= 15;
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.BeginVertical();
                        {
                            Draw();
                        }
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.Space(15, false);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space(100);
                }
                EditorGUILayout.EndScrollView();
            }
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

            void Draw()
            {
                GUILayout.Label("File Viewer", EditorStyles.boldLabel);
                
                DrawDebug();

                DrawStageSelect();

                DrawSpawnDataSelect();



                #region GUI
                
                void DrawDebug()
                {
                    EditorGUILayout.ObjectField(data, typeof(StageEditorData), false);
                    EditorGUILayout.ObjectField(setting, typeof(StageEditorSetting), false);

                    DrawStageList();

                    DrawPrefabList();
                    
                    TitleHeaderLabel("Select");
                    BeginNewTab();
                    {
                        DrawSelectInfo();
                    }
                    EndNewTab();


                    void DrawStageList()
                    {
                        TitleHeaderLabel("Stage");
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

                    void DrawPrefabList()
                    {
                        TitleHeaderLabel("Prefab");
                        
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
                    
                    void DrawSelectInfo()
                    {
                        TitleHeaderLabel("Stage");
                        EditorGUILayout.ObjectField(data.selectedStage, typeof(Stage), false);

                        if (data.selectedStage == null)
                        {
                            WarningLabel("SelectedStage is null");
                            return;
                        }

                        DrawSelectedEnemyData();
                        


                        void DrawSelectedEnemyData()
                        {
                            TitleHeaderLabel("Enemy");
                            if (data.selectedEnemyData == null)
                            {
                                WarningLabel("SelectedEnemyData is null");
                                return;
                            }

                            //Spawn Data
                            EditorGUILayout.ObjectField("Spawn Data", data.selectedEnemyData.spawnData, typeof(EnemySpawnData), false);

                            //EditorGUI
                            if (data.selectedEnemyData.unSafeEditorGUI == null)
                            {
                                EditorGUILayout.TextField("EditorGUI", "None");
                            }
                            else EditorGUILayout.TextField("EditorGUI", data.selectedEnemyData.unSafeEditorGUI.GetType().Name);

                            //Prefab
                            EditorGUILayout.ObjectField("Prefab", data.selectedEnemyData.prefab, typeof(GameObject), false);
                            if (data.selectedEnemyData.prefab == null)
                            {
                                EditorGUILayout.TextField("Prefab Type", "None");
                            }
                            else EditorGUILayout.TextField("Prefab Type", data.selectedEnemyData.prefabType.Name);
                        }
                    }
                }

                void DrawStageSelect()
                {
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
                        if (GUI.Button(GUILayoutUtility.GetRect(setting.buttonWidth, setting.buttonHeight, GUILayout.ExpandWidth(false)).GetAddY(2), "Refresh"))
                        {
                            data.RefreshStageArray();
                            data.RefreshTimeFoldout();
                        }
                    }
                    GUILayout.EndHorizontal();
                }

                void DrawSpawnDataSelect()
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("Spawn Data", EditorStyles.boldLabel);
                    if (data.enemyList == null || data.enemyList.Count < 1)
                    {
                        WarningLabel("EnemySpawnData is Empty");
                        return;
                    }
                    // Enemy List
                    Rect selectRect = Rect.zero;
                    Rect headerRect = Rect.zero;
                    bool isSelectHideByHeader = false;
                    int elementCount = 0;
                    float prevTime = -1;
                    bool foldout = false;
                    for (int i = 0; i < data.enemyList.Count; i++)
                    {
                        EditorEnemyData enemyData = data.enemyList[i];

                        if (enemyData.spawnData.spawnTime != prevTime)
                        {
                            if (foldout == false) LateCloseHeader();

                            if (data.timeFoldout.TryGetValue(enemyData.spawnData.spawnTime, out foldout) == false)
                            {
                                data.timeFoldout.Add(enemyData.spawnData.spawnTime, true);
                                foldout = true;
                            }

                            if (foldout)
                            {
                                OpenHeader(enemyData, i);
                            }
                            else
                            {
                                CloseHeader(enemyData, i);
                            }
                            elementCount = 0;
                        }
                        else
                        {
                            elementCount++;

                            if (foldout)
                            {
                                OpenElement(enemyData, i);
                            }
                            else
                            {
                                CloseElement(enemyData, i);
                            }
                        }

                        prevTime = enemyData.spawnData.spawnTime;
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
                    void OpenHeader(EditorEnemyData enemyData, int i)
                    {
                        EditorGUILayout.Space(5);

                        GUILayout.BeginHorizontal();
                        {
                            //Time Label
                            Rect timeRect = GUILayoutUtility.GetRect(setting.timeWidth, EditorGUIUtility.singleLineHeight, GUILayout.ExpandWidth(false));
                            EditorGUI.LabelField(timeRect, enemyData.spawnData.spawnTime.ToString("F1"));

                            if (EventUtility.MouseDown(0) && timeRect.Contains(e.mousePosition))
                            {
                                data.timeFoldout[enemyData.spawnData.spawnTime] = !foldout;
                                Repaint();
                            }

                            //Object Field
                            EditorGUILayout.ObjectField(enemyData.spawnData, typeof(EnemyData), false, GUILayout.Height(setting.buttonHeight));
                            Rect objectRect = GUILayoutUtility.GetLastRect();

                            //Select Button
                            if (GUI.Button(GUILayoutUtility.GetRect(setting.buttonWidth, setting.buttonHeight, GUILayout.ExpandWidth(false)).GetAddY(2), "Select"))
                            {
                                SelectEnemyData(i);
                            }

                            ShowHasData(enemyData, objectRect);

                            headerRect = objectRect;
                            if (enemyData == data.selectedEnemyData)
                            {
                                selectRect = headerRect;
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    void CloseHeader(EditorEnemyData enemyData, int i)
                    {
                        OpenHeader(enemyData, i);
                    }

                    void OpenElement(EditorEnemyData enemyData, int i)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            Rect timeRect = GUILayoutUtility.GetRect(setting.timeWidth, EditorGUIUtility.singleLineHeight, GUILayout.ExpandWidth(false));

                            EditorGUILayout.ObjectField(enemyData.spawnData, typeof(EnemyData), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                            Rect objectRect = GUILayoutUtility.GetLastRect();

                            if (GUI.Button(GUILayoutUtility.GetRect(setting.buttonWidth, setting.buttonHeight, GUILayout.ExpandWidth(false)).GetAddY(2), "Select"))
                            {
                                GUI.FocusControl(null);
                                data.SelectEnemyData(i);
                            }

                            ShowHasData(enemyData, objectRect);

                            if (enemyData == data.selectedEnemyData)
                            {
                                selectRect = objectRect.GetSetWidth(area.width - timeRect.width);
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    void CloseElement(EditorEnemyData enemyData, int i)
                    {
                        if (enemyData == data.selectedEnemyData)
                        {
                            selectRect = headerRect;
                            isSelectHideByHeader = true;
                        }
                    }

                    void ShowHasData(EditorEnemyData enemyData, Rect objectRect)
                    {
                        if (enemyData.unSafeEditorGUI != null)
                        {
                            Rect E = objectRect.GetAddX(objectRect.width - objectRect.height).GetSetWidth(objectRect.height);
                            CustomGUI.DrawSquare(E, setting.hasEditorBackGroundColor);

                            EditorStyles.boldLabel.alignment = TextAnchor.MiddleCenter;
                            GUI.Label(E, " E", EditorStyles.boldLabel);
                            EditorStyles.boldLabel.alignment = TextAnchor.MiddleLeft;
                        }
                    }
                }
                
                #endregion
            }
            
            void SelectEnemyData(int index)
            {
                GUI.FocusControl(null);
                data.SelectEnemyData(index);
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

                if (data.selectedEnemyData != null)
                {
                    #region SpawnPrefab

                    List<GameObject> prefabList = data.GetPrefabList(data.selectedEnemyData.spawnData);

                    string[] selectablePrefabNameArray;
                    if (prefabList == null)
                    {
                        WarningLabel("Cannot Found Type: " + data.selectedEnemyData.spawnData.GetType());
                    }
                    else
                    {
                        selectablePrefabNameArray = prefabList.Select(prefab => prefab.name).ToArray();

                        int prefabIndexInput = EditorGUILayout.Popup(
                                data.selectedEnemyData.spawnData.prefabIndex, selectablePrefabNameArray
                            );
                        if (prefabIndexInput != data.selectedEnemyData.spawnData.prefabIndex)
                        {
                            data.selectedEnemyData.SelectPrefab(prefabIndexInput);
                        }
                    }
                    #endregion

                    #region SpawnTime
                    float spawnTimeInput = EditorGUILayout.FloatField("Spawn Time", data.selectedEnemyData.spawnData.spawnTime);
                    if (spawnTimeInput != data.selectedEnemyData.spawnData.spawnTime)
                    {
                        if (spawnTimeInput >= 0) data.selectedEnemyData.spawnData.spawnTime = spawnTimeInput;
                        else data.selectedEnemyData.spawnData.spawnTime = 0;

                        data.SortSelectedEnemyData();
                    }
                    #endregion

                    if (data.selectedEnemyData.editorGUI != null) data.selectedEnemyData.editorGUI.DrawInspectorGUI(data.selectedEnemyData);
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
            if (e.type == EventType.Repaint)
            {
                if (previewRender == null)
                {
                    Debug.LogError("PreviewRender is null");
                }
                else if (previewRect.size.x != 0 && previewRect.size.y != 0)
                {
                    previewRender.BeginStaticPreview(new Rect(Vector2.zero, previewRect.size));

                    data.selectedEnemyData?.editorGUI.Render(previewRender, data.selectedEnemyData);

                    previewRender.Render();

                    prefabPreview = previewRender.EndStaticPreview();
                }
            }

            GUI.DrawTexture(previewRect, prefabPreview ?? Texture2D.whiteTexture);

            Vector2Int cellCount = 3 * Vector2Int.one + new Vector2Int(
                Mathf.FloorToInt(previewRect.width / data.cellSize),
                Mathf.FloorToInt(previewRect.height / data.cellSize)
            );
            Vector2 offset = new Vector2((data.previewPos.x) % data.cellSize, (data.previewPos.y) % data.cellSize);
            Vector2 start = new Vector2(Mathf.Floor(previewRect.x / data.cellSize) * data.cellSize, Mathf.Floor(previewRect.y / data.cellSize) * data.cellSize);
            CustomGUI.DrawOpenGrid(start + offset - data.cellSize * Vector2.one, cellCount, data.cellSize, setting.previewOutGridColor);
            CustomGUI.DrawCloseGrid(data.previewPos + data.cellSize * new Vector2(-0.5f * Window.GameWidth, -Window.GameHeight), new Vector2Int(Window.GameWidth, Window.GameHeight), data.cellSize, setting.previewGameGridColor);

            if (data.selectedEnemyData != null && data.selectedEnemyData.editorGUI != null)
            {
                DrawEnemyGizmos();
                DrawTimeLine();
            }
            void DrawEnemyGizmos()
            {
                for (int i = data.selectedEnemyDataIndex + 1; i < data.enemyList.Count; i++)
                {
                    EditorEnemyData enemyData = data.enemyList[i];
                    if (data.selectedEnemyData.spawnData.spawnTime == enemyData.spawnData.spawnTime)
                    {
                        enemyData.editorGUI.DrawSameTimeEnemyDataGizmos(enemyData);
                    }
                    else break;
                }
                for (int i = data.selectedEnemyDataIndex - 1; i >= 0; i--)
                {
                    EditorEnemyData enemyData = data.enemyList[i];
                    if (data.selectedEnemyData.spawnData.spawnTime == enemyData.spawnData.spawnTime)
                    {
                        enemyData.editorGUI.DrawSameTimeEnemyDataGizmos(enemyData);
                    }
                    else break;
                }
                data.selectedEnemyData.editorGUI.DrawSelectedEnemyDataGizmos(data.selectedEnemyData);
            }
        }

        void DrawTimeLine()
        {
            float timeLineStart = data.fileViewerLinePosX + setting.timeHorizontalSpace;
            float timeLineEnd = (position.width - data.inspectorLinePosX) - setting.timeHorizontalSpace;

            if (timeLineStart > timeLineEnd) return;

            float timeLineY = position.height - setting.timeBottomSpace;

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
            if (playTime != -1)
            {
                Vector2 playTimeLineScreenPos = GetTimeScreenPos(playTime);
                Handles.color = Color.green;
                Handles.DrawLine(
                    playTimeLineScreenPos.GetAddY(setting.timeLengthFieldOffsetY),
                    playTimeLineScreenPos.GetAddY(-setting.timeLengthFieldOffsetY)
                );
                Handles.color = Color.white;
            }

            for (int i = 0; i < data.enemyList.Count; i++)
            {
                if (i == data.selectedEnemyDataIndex) continue;

                DrawTimeLineMarker(
                    data.enemyList[i].spawnData.spawnTime,
                    setting.enemySpawnTimeColor
                );
            }
            DrawTimeLineMarker(
                data.selectedEnemyData.spawnData.spawnTime,
                setting.selectEnemySpawnTimeColor
            );

            void DrawTimeLineMarker(float time, SquareColor color)
            {
                Rect timeLineMarkerRect = new Rect();
                timeLineMarkerRect.size = Vector2.one * setting.timeCubeSize;
                timeLineMarkerRect.position = GetTimeScreenPos(time) - 0.5f * timeLineMarkerRect.size;

                CustomGUI.DrawSquare(timeLineMarkerRect, color);
            }
            Rect timeLengthFieldRect = new Rect();
            timeLengthFieldRect.position = new Vector2(timeLineEnd, timeLineY - (setting.timeLengthFieldSize.y + setting.timeLengthFieldOffsetY));
            timeLengthFieldRect.size = setting.timeLengthFieldSize;

            data.timeLength = EditorGUI.DelayedFloatField(timeLengthFieldRect, data.timeLength);
            if (data.enemyList.Count >= 1)
            {
                float lastTime = data.enemyList[^1].spawnData.spawnTime;
                if (lastTime > data.timeLength) data.timeLength = lastTime;
            }
        }

        #endregion

        #region Utility

        void Input()
        {
            if (data.selectedEnemyData != null) data.selectedEnemyData.editorGUI.Event();
            
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
                        case KeyCode.UpArrow:
                            int prevIndex = data.selectedEnemyDataIndex - 1;
                            if (0 <= prevIndex)
                            {
                                data.SelectEnemyData(prevIndex);
                                Repaint();
                            }
                            break;

                        case KeyCode.DownArrow:
                            int nextIndex = data.selectedEnemyDataIndex + 1;
                            if (data.selectedEnemyDataIndex != -1 && nextIndex < data.enemyList.Count)
                            {
                                data.SelectEnemyData(nextIndex);
                                Repaint();
                            }
                            break;
                        case KeyCode.Comma:
                            if (data.selectedEnemyData != null && 0 < data.selectedEnemyData.spawnData.spawnTime)
                            { 
                                data.selectedEnemyData.spawnData.spawnTime -= data.timeMoveSnap;
                                if (data.selectedEnemyData.spawnData.spawnTime < 0)
                                {
                                    data.selectedEnemyData.spawnData.spawnTime = 0;
                                }
                                else data.selectedEnemyData.spawnData.spawnTime = Mathf.Ceil(data.selectedEnemyData.spawnData.spawnTime / data.timeMoveSnap) * data.timeMoveSnap;
                                
                                data.SortSelectedEnemyData();
                                Repaint();
                            }
                            break;
                        case KeyCode.Period:
                            if (data.selectedEnemyData != null)
                            {
                                data.selectedEnemyData.spawnData.spawnTime += data.timeMoveSnap;
                                data.selectedEnemyData.spawnData.spawnTime = Mathf.Floor(data.selectedEnemyData.spawnData.spawnTime / data.timeMoveSnap) * data.timeMoveSnap;
                                
                                if (data.selectedEnemyData.spawnData.spawnTime > data.timeLength)
                                {
                                    data.timeLength = data.selectedEnemyData.spawnData.spawnTime;
                                }

                                data.SortSelectedEnemyData();
                                Repaint();
                            }
                            break;
                    }
                    break;
            }

            data.selectedEnemyData?.editorGUI.LateEvent();

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
            Box previewBox = new Box(
                new Vector2(Window.GameWidth, Window.GameHeight) * data.cellSize,
                 new Vector2(0, -0.5f * Window.GameHeight * data.cellSize)
            );
            if (previewBox.IsExit(data.previewPos, position.height, 0, (position.width - data.inspectorLinePosX), data.fileViewerLinePosX, out Vector2 previewContact))
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
            if (previewRect == null) previewRect = new Rect();
            previewRect.position = new Vector2(data.fileViewerLinePosX, 0);
            previewRect.size = new Vector2(position.width - (data.inspectorLinePosX + data.fileViewerLinePosX), position.height);
        }

        #endregion

        #region UI Utility

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
            EditorGUILayout.Space(10, false);
            EditorGUILayout.BeginVertical(GUILayout.Width(areaWidth));
        }
        void EndNewTab()
        {
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        #endregion
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
        return data.previewPos + worldPos.GetMultiplyY(-1) * data.cellSize;
    }
    public static Vector2 ScreenToWorldPoint(Vector2 screenPos)
    {
        return ((screenPos - data.previewPos) / data.cellSize).GetMultiplyY(-1);
    }
    public static Vector2 GetTimeScreenPos(float time)
    {
        float timeLineStart = data.fileViewerLinePosX + setting.timeHorizontalSpace;
        float timeLineEnd = (instance.position.width - data.inspectorLinePosX) - setting.timeHorizontalSpace;
        
        if (timeLineStart > timeLineEnd) return -Vector2.one;
        
        float timeLineY = instance.position.height - setting.timeBottomSpace;
        float timeRatio = time / data.timeLength;

        return new Vector2(Mathf.Lerp(timeLineStart, timeLineEnd, timeRatio), timeLineY);
    }
}