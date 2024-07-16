#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

[InitializeOnLoad]
public class StageEditor : EditorWindow
{
    public static StageEditor instance;
    public static StageEditorData data;
    public static StageEditorSetting setting;
    
    static Event e => Event.current;
    static Window Win => Window.instance;

    public Rect fileViewerRect = new Rect();
    public Rect inspectorRect = new Rect();
    public Rect previewRect = new Rect();
    
    public bool debug = false;

    FloatingAreaManager floatingArea;
    public Dictionary<Type, EnemyEditorGUI> enemyEditorGUIDictionary = new Dictionary<Type, EnemyEditorGUI>();

    public float fileViewerScroll;

    enum HoldType { None, Preview, FileViewerLine, InspectorLine }
    HoldType hold = HoldType.None;

    Vector2 offset;
    float prevScreenWidth;

    public PreviewRenderUtility previewRender = null;

    Rect selectRect = default;

    Rect enemySpawnDataReNameFloatingAreaHeader = default;

    #region Event

    static StageEditor()
    {
        Debug.Log("Initialize");
        
        EditorApplication.wantsToQuit += OnQuit;
    }

    [MenuItem("Window/StageEditor")]
    public static void CreateWindow()
    {
        Debug.Log("Create");

        instance = (StageEditor)GetWindow(typeof(StageEditor));

        instance.Show();
    }

    Texture _outputTexture;

    static bool OnQuit()
    {
        Debug.Log("Quit");

        data.SelectEnemyData(-1);

        return true;
    }
    void OnValidate()
    {
        Debug.Log("Validate");
        
        instance = this;
    }
    void OnEnable()
    {
        Debug.Log("Enable");
        
        data = (StageEditorData)EditorResources.Load<ScriptableObject>("StageEditor/Data.asset");
        setting = (StageEditorSetting)EditorResources.Load<ScriptableObject>("StageEditor/Setting.asset");
    
        wantsMouseEnterLeaveWindow = true;
        floatingArea = new FloatingAreaManager();

        PreviewInit();

        Refresh();
    }

    private void OnDisable()
    {
        Debug.Log("Disable");
        
        Apply();
        ClearPreview();
    }
    public void Apply()
    {
        data.ApplyToStage();
        EditorUtility.SetDirty(data);
        EditorUtility.SetDirty(setting);
    }
    public void Refresh()
    {
        RefreshEnemyEditors();
        enemyEditorGUIDictionary.Clear();

        PreviewInit();

        data.RefreshStageArray();
        data.RefreshTimeFoldout();

        selectRect = default;

        floatingArea.area = null;
        enemySpawnDataReNameFloatingAreaHeader = default;
    }

    #endregion

    #region EnemyEditorGUI

    public void RefreshEnemyEditors()
    {
        foreach (EnemyEditorGUI editorGUI in enemyEditorGUIDictionary.Values)
        {
            editorGUI.Refresh();
        }
    }
    public EnemyEditorGUI GetEnemyEditor(EnemySpawnData spawnData) => GetEnemyEditor(spawnData.EditorType);
    public EnemyEditorGUI GetEnemyEditor(Type editorType)
    {
        EnemyEditorGUI editorInstance;

        if (enemyEditorGUIDictionary.TryGetValue(editorType, out editorInstance) == false)
        {
            editorInstance = (EnemyEditorGUI)Activator.CreateInstance(editorType);
            enemyEditorGUIDictionary.Add(editorType, editorInstance);
        }
        return editorInstance;
    }

    #endregion

    #region DrawPreview

    public void ClearPreview()
    {
        data.sameTimeEnemyList.Clear();
        previewRender.Cleanup();
    }
    public RenderTexture CreatePreviewTexture(Rect rect)
    {
        previewRender.camera.transform.position = VectorUtility.SetZ(((Vector3)StageEditor.ScreenToWorldPoint(rect.GetPivotPos(Anchor.MiddleCenter))), -10);
        previewRender.camera.orthographicSize = 0.5f * (rect.height / data.cellSize);

        previewRender.BeginPreview(rect, GUIStyle.none);

        //previewRender.lights[0].transform.localEulerAngles = new Vector3(30, 30, 0);
        previewRender.lights[0].intensity = 2;

        previewRender.camera.Render();

        return (RenderTexture)previewRender.EndPreview();
    }
    public void PreviewInit()
    {
        if (previewRender != null) ClearPreview();

        previewRender = new PreviewRenderUtility(true);

        GC.SuppressFinalize(previewRender);

        var camera = previewRender.camera;
        Camera gameCam = CameraController.instance.cam;
        camera.orthographic = gameCam.orthographic;
        camera.orthographicSize = 1;

        camera.transform.rotation = gameCam.transform.rotation;

        camera.clearFlags = gameCam.clearFlags;
        camera.backgroundColor = StageEditor.setting.previewBackGroundColor;
        camera.cullingMask = gameCam.cullingMask;

        camera.fieldOfView = gameCam.fieldOfView;
        camera.nearClipPlane = gameCam.nearClipPlane;
        camera.farClipPlane = gameCam.farClipPlane;
    }

    #endregion

    
    void OnGUI()
    {
        #region Init

        Handles.color = Color.white;
        if(floatingArea.area != null) floatingArea.area.backGroundColor = setting.floatingAreaBackGroundColor;

        #endregion

        floatingArea.EventListen(e);

        RefreshPreviewRect();
        
        if(previewRect.width != 0)
        {
            DrawPreview();
        }

        Input();
        
        RefreshFileViewerRect();
        RefreshInspectorRect();

        if (data.fileViewerLinePosX != 0) DrawFileViewerGUI();
        if (data.inspectorLinePosX != 0) DrawInspectorGUI();

        floatingArea.Draw();

        if (position.width != prevScreenWidth)
        {
            OnScreanWidthResized();
            prevScreenWidth = position.width;
        }

        void Input()
        {
            if (data.selectedEnemyData != null) data.selectedEnemyData.editorGUI.Event();

            if (e.isScrollWheel)
            {
                if (previewRect.Contains(e.mousePosition))
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
                            if (data.selectedEnemyDataIndex != -1 && nextIndex < data.editorEnemySpawnDataList.Count)
                            {
                                data.SelectEnemyData(nextIndex);
                                Repaint();
                            }
                            break;
                        case KeyCode.Comma:
                            if (data.selectedEnemyData != null && 0 < data.selectedEnemyData.spawnData.spawnTime)
                            {
                                data.selectedEnemyData.spawnData.spawnTime -= setting.timeMoveSnap;
                                if (data.selectedEnemyData.spawnData.spawnTime < 0)
                                {
                                    data.selectedEnemyData.spawnData.spawnTime = 0;
                                }
                                else data.selectedEnemyData.spawnData.spawnTime = Mathf.Ceil(data.selectedEnemyData.spawnData.spawnTime / setting.timeMoveSnap) * setting.timeMoveSnap;

                                data.SortSelectedEnemyData();
                                Repaint();
                            }
                            break;
                        case KeyCode.Period:
                            if (data.selectedEnemyData != null)
                            {
                                data.selectedEnemyData.spawnData.spawnTime += setting.timeMoveSnap;
                                data.selectedEnemyData.spawnData.spawnTime = Mathf.Floor(data.selectedEnemyData.spawnData.spawnTime / setting.timeMoveSnap) * setting.timeMoveSnap;

                                if (data.selectedEnemyData.spawnData.spawnTime > data.selectedStage.timeLength)
                                {
                                    data.selectedStage.timeLength = data.selectedEnemyData.spawnData.spawnTime;
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
                fileViewerScroll = EditorGUILayout.BeginScrollView(
                    new Vector2(0, fileViewerScroll), 
                    false, 
                    true, 
                    GUIStyle.none, 
                    GUI.skin.verticalScrollbar, 
                    GUIStyle.none
                ).y;
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

            Rect applyButtonRect = new Rect();
            applyButtonRect.size = new Vector2(setting.buttonWidth, setting.buttonHeight);
            applyButtonRect.position = area.position + area.size - applyButtonRect.size;

            if (GUI.Button(applyButtonRect, "Apply"))
            {
                Apply();
            }

            if (GUI.Button(applyButtonRect.AddX(-applyButtonRect.width), "Refresh"))
            {
                Refresh();
            }
            
            void Draw()
            {
                GUILayout.Label("File Viewer", EditorStyles.boldLabel);
                
                DrawDebug();

                DrawStageSelect();
                bool createButtonDown = GUILayout.Button("Create");
                if (e.type == EventType.Repaint) floatingArea.SetRect(GUILayoutUtility.GetLastRect().AddPos(area.position.AddY(5 - fileViewerScroll)));

                if (createButtonDown)
                {
                    if (floatingArea.area == null)
                    {
                        floatingArea.Create(new CreateNewEnemyFloatingArea());
                    }
                    else floatingArea.Destroy();
                }
                DrawSpawnDataList();



                #region GUI
                
                void DrawDebug()
                {
                    EditorGUILayout.ObjectField(data, typeof(StageEditorData), false);
                    EditorGUILayout.ObjectField(setting, typeof(StageEditorSetting), false);

                    DrawSelectStage();

                    DrawStageList();

                    DrawPrefabList();
                    
                    CustomGUILayout.TitleHeaderLabel("Select");
                    CustomGUILayout.BeginNewTab();
                    {
                        DrawSelectInfo();
                    }
                    CustomGUILayout.EndNewTab();

                    CustomGUILayout.TitleHeaderLabel(title: "Preview Object");
                    CustomGUILayout.BeginNewTab();
                    {
                        DrawPreviewObject();
                    }
                    CustomGUILayout.EndNewTab();

                    void DrawSelectStage()
                    {
                        CustomGUILayout.TitleHeaderLabel("Selected Stage");
                        
                        if (data.selectedStage == null)
                        {
                            CustomGUILayout.WarningLabel("SelectedStage is null");
                            return;
                        }

                        CustomGUILayout.BeginNewTab();
                        {
                            CustomGUILayout.TitleHeaderLabel("Prefab");
                            if (data.selectedStage.enemyPrefabs == null) CustomGUILayout.WarningLabel("EnemyPrefabs is null");
                            else if (data.selectedStage.enemyPrefabs.Length == 0) CustomGUILayout.WarningLabel("EnemyPrefabs is Empty");
                            else
                            {
                                foreach (GameObject prefab in data.selectedStage.enemyPrefabs)
                                {
                                    EditorGUILayout.ObjectField(prefab, typeof(GameObject), false);
                                }
                            }

                            CustomGUILayout.TitleHeaderLabel("Spawn Data");
                            if (data.selectedStage.enemySpawnDataArray == null)
                            {
                                CustomGUILayout.WarningLabel("EnemySpawnDataArray is null");
                            }
                            else if (data.selectedStage.enemySpawnDataArray.Length == 0) CustomGUILayout.WarningLabel("EnemySpawnDataArray is Empty");
                            else
                            {
                                float prevTime = -1;
                                foreach (EnemySpawnData enemySpawnData in data.selectedStage.enemySpawnDataArray)
                                {
                                    Rect fieldRect = GUILayoutUtility.GetRect(
                                        0,
                                        EditorGUIUtility.singleLineHeight,
                                        GUILayout.ExpandWidth(true)
                                    );

                                    //Time Label
                                    Rect timeRect = fieldRect.SetWidth(setting.timeWidth);
                                    if (enemySpawnData.spawnTime != prevTime)
                                    {
                                        EditorGUI.LabelField(
                                            timeRect,
                                            enemySpawnData.spawnTime.ToString("F1")
                                        );
                                        prevTime = enemySpawnData.spawnTime;
                                    }

                                    //Object Field
                                    Rect objectRect = RectUtility.AddWidth(fieldRect.AddX(timeRect.width), -timeRect.width);
                                    EditorGUI.ObjectField(
                                        objectRect, 
                                        enemySpawnData, 
                                        typeof(EnemySpawnData), 
                                        false
                                    );
                                }
                                
                            }
                        }
                        CustomGUILayout.EndNewTab();
                    }

                    void DrawStageList()
                    {
                        CustomGUILayout.TitleHeaderLabel("Stage");
                        CustomGUILayout.BeginNewTab();
                        {
                            CustomGUILayout.TitleHeaderLabel("Stage List");
                            if (data.stageArray == null) CustomGUILayout.WarningLabel("Stage Array is null");
                            else if (data.stageArray.Length < 1) CustomGUILayout.WarningLabel("Stage Array is Empty");
                            else
                            {
                                foreach (Stage stage in data.stageArray)
                                {
                                    EditorGUILayout.ObjectField(stage, typeof(Stage), false);
                                }
                            }
                        }
                        CustomGUILayout.EndNewTab();

                    }

                    void DrawPrefabList()
                    {
                        CustomGUILayout.TitleHeaderLabel("Prefab");
                        
                        if (data.prefabLists == null || data.prefabLists.Count == 0)
                        {
                            CustomGUILayout.WarningLabel("Empty PrefabList");
                        }
                        else
                        {
                            CustomGUILayout.BeginNewTab();
                            {
                                for (int listIndex = 0; listIndex < data.prefabLists.Count; listIndex++)
                                {
                                    List<GameObject> curList = data.prefabLists[listIndex];
                                    Type listType = data.prefabTypeList[listIndex];

                                    if (curList.Count < 1)
                                    {
                                        CustomGUILayout.WarningLabel("Empty List");
                                        continue;
                                    }
                                    CustomGUILayout.UnderBarTitleText(listType.Name);

                                    for (int elementIndex = 0; elementIndex < curList.Count; elementIndex++)
                                    {
                                        EditorGUILayout.ObjectField(curList[elementIndex], typeof(GameObject), false);
                                    }
                                }
                            }
                            CustomGUILayout.EndNewTab();
                        }
                    }
                    
                    void DrawSelectInfo()
                    {
                        CustomGUILayout.TitleHeaderLabel("Stage");
                        EditorGUILayout.ObjectField(data.selectedStage, typeof(Stage), false);

                        if (data.selectedStage == null)
                        {
                            CustomGUILayout.WarningLabel("SelectedStage is null");
                            return;
                        }

                        DrawSelectedEnemyData();
                        


                        void DrawSelectedEnemyData()
                        {
                            CustomGUILayout.TitleHeaderLabel("Enemy");
                            if (data.selectedEnemyData == null)
                            {
                                CustomGUILayout.WarningLabel("SelectedEnemyData is null");
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

                    void DrawPreviewObject()
                    {
                        for (int categoryIndex = 0; categoryIndex < enemyEditorGUIDictionary.Count; categoryIndex++)
                        {
                            foreach (EnemyEditorGUI category in enemyEditorGUIDictionary.Values)
                            {
                                string categoryName = category.GetType().Name;
                                CustomGUILayout.UnderBarTitleText(categoryName);

                                if (category.instance.Count == 0)
                                {
                                    CustomGUILayout.WarningLabel("Empty List");
                                }
                                else
                                {
                                    foreach (EditorEnemyData key in category.instance.Keys)
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.ObjectField(key.spawnData, typeof(EnemySpawnData), true);
                                        EditorGUILayout.ObjectField(category.instance[key], typeof(GameObject), true);
                                        EditorGUILayout.EndHorizontal();
                                    }
                                }
                            }
                        }
                    }
                }

                void DrawStageSelect()     
                {
                        if (data.stageArray == null)
                        {
                            CustomGUILayout.WarningLabel("StageArray is null");
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
                }

                void DrawSpawnDataList() 
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("Spawn Data", EditorStyles.boldLabel);
                    if (data.editorEnemySpawnDataList == null || data.editorEnemySpawnDataList.Count < 1)
                    {
                        CustomGUILayout.WarningLabel("EnemySpawnData is Empty");
                        return;
                    }
                    // Enemy List
                    Rect headerRect = Rect.zero;
                    bool isSelectHideByHeader = false;
                    int elementCount = 0;
                    float prevTime = -1;
                    bool foldout = false;
                    for (int i = 0; i < data.editorEnemySpawnDataList.Count; i++)
                    {
                        EditorEnemyData enemyData = data.editorEnemySpawnDataList[i];

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
                    if (selectRect != default)
                    {
                        if (isSelectHideByHeader)
                        {
                            HidedSelect();
                        }
                        else Select();
                    }
                    if (foldout == false) LateCloseHeader();

                    if (enemySpawnDataReNameFloatingAreaHeader != default && e.type == EventType.Repaint)
                    {
                        floatingArea.SetRect(enemySpawnDataReNameFloatingAreaHeader.AddY( -fileViewerScroll));
                    }

                    void HidedSelect()
                    {
                        Vector2 p1 = headerRect.position.AddY(headerRect.height);
                        Vector2 p2 = p1.AddX(headerRect.width);
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
                            Vector2 p1 = headerRect.position.AddY(headerRect.height);
                            Vector2 p2 = p1.AddX(headerRect.width);
                            Handles.color = Color.white;
                            Handles.DrawLine(p1, p2);

                            EditorGUILayout.Space(5);
                        }
                    }
                    void OpenHeader(EditorEnemyData editorEnemyData, int i)
                    {
                        EditorGUILayout.Space(5);

                        Rect fieldRect = GUILayoutUtility.GetRect(
                            area.width, 
                            EditorGUIUtility.singleLineHeight, 
                            GUILayout.ExpandWidth(true)
                        );

                        //Time Label
                        Rect timeRect = fieldRect.SetWidth(setting.timeWidth);
                        EditorGUI.LabelField(timeRect, editorEnemyData.spawnData.spawnTime.ToString("F1"));

                        if (EventUtility.MouseDown(0) && timeRect.Contains(e.mousePosition))
                        {
                            data.timeFoldout[editorEnemyData.spawnData.spawnTime] = !foldout;
                            Repaint();
                        }

                        //Object Field
                        Rect objectRect = RectUtility.AddWidth(fieldRect.AddX(timeRect.width), -(timeRect.width + setting.buttonWidth));
                        AddMenu(
                            objectRect, 
                            data.editorEnemySpawnDataList[i]
                        );
                        EditorGUI.ObjectField(
                            objectRect,
                            "",
                            editorEnemyData.spawnData,
                            typeof(EnemySpawnData),
                            false
                        );
                        DrawMarker(objectRect, editorEnemyData);

                        //Select Button
                        Rect selectButtonRect = objectRect.AddX(objectRect.width).SetWidth(setting.buttonWidth);
                        if (GUI.Button(selectButtonRect, "Select"))
                        {
                            SelectEnemyData(i);
                        }

                        headerRect = objectRect;
                        if (editorEnemyData == data.selectedEnemyData)
                        {
                            selectRect = headerRect;
                        }
                    }
                    void CloseHeader(EditorEnemyData enemyData, int i)
                    {
                        OpenHeader(enemyData, i);
                    }

                    void OpenElement(EditorEnemyData editorEnemyData, int i)
                    {
                        Rect fieldRect = GUILayoutUtility.GetRect(
                            area.width,
                            EditorGUIUtility.singleLineHeight,
                            GUILayout.ExpandWidth(true)
                        );

                        //Time Label
                        Rect timeRect = fieldRect.SetWidth(setting.timeWidth);

                        //Object Field
                        Rect objectRect = RectUtility.AddWidth(fieldRect.AddX(timeRect.width), enemySpawnDataReNameFloatingAreaHeader.height - (timeRect.width + setting.buttonWidth));
                        AddMenu(
                            objectRect,
                            data.editorEnemySpawnDataList[i]
                        );
                        EditorGUI.ObjectField(
                            objectRect,
                            "",
                            editorEnemyData.spawnData,
                            typeof(EnemySpawnData),
                            false
                        );
                        DrawMarker(objectRect, editorEnemyData);

                        //Select Button
                        Rect selectButtonRect = objectRect.AddX(objectRect.width).SetWidth(setting.buttonWidth);
                        if (GUI.Button(selectButtonRect, "Select"))
                        {
                            SelectEnemyData(i);
                        }

                        if (editorEnemyData == data.selectedEnemyData)
                        {
                            selectRect = RectUtility.SetY(headerRect, objectRect.y);
                        }
                    }
                    void CloseElement(EditorEnemyData enemyData, int i)
                    {
                        if (enemyData == data.selectedEnemyData)
                        {
                            selectRect = headerRect;
                            isSelectHideByHeader = true;
                        }
                    }
                    void DrawMarker(Rect objectRect, EditorEnemyData editorEnemyData)
                    {
                        Rect r1 = new Rect();
                        r1 = r1.SetSize(objectRect.height - 3, objectRect.height - 3);
                        r1 = r1.SetPos(objectRect.GetPivotPos(Anchor.TopRight) + new Vector2(-r1.width, 1.5f));

                        Rect r2 = r1.AddX(-r1.width);

                        CustomGUI.DrawSquare(r2.SetWidth(r2.width * 2), setting.hasEditorBackGroundColor);
                        
                        GUIStyle textStyle = new GUIStyle(EditorStyles.boldLabel);
                        EditorStyles.boldLabel.alignment = TextAnchor.MiddleCenter;
                        {
                            //Editor
                            if (editorEnemyData.unSafeEditorGUI == null) textStyle.normal.textColor = Color.gray;
                            else textStyle.normal.textColor = Color.white;

                            GUI.Label(r1, " E", textStyle);

                            //Prefab
                            if (editorEnemyData.prefab == null) textStyle.normal.textColor = Color.red;
                            else textStyle.normal.textColor = Color.white;

                            GUI.Label(r2, " P", textStyle);
                        }
                        EditorStyles.boldLabel.alignment = TextAnchor.MiddleLeft;
                    }
                }
                
                #endregion
            }
            
            void AddMenu(Rect rect, EditorEnemyData editorEnemyData)
            {
                if (EventUtility.MouseDown(1) && rect.Contains(e.mousePosition))
                {
                    e.Use();
                    GenericMenu menu = new GenericMenu();

                    /*menu.AddItem(
                        new GUIContent("Rename"),
                        false,
                        () =>
                        {
                            enemySpawnDataReNameFloatingAreaHeader = rect.AddPos(area.position);
                            floatingArea.Create(
                                new TextFloatingArea(
                                    editorEnemyData.spawnData.name,

                                    applyEvent: (name) => {
                                        AssetDatabase.RenameAsset(
                                            AssetDatabase.GetAssetPath(
                                                editorEnemyData.spawnData
                                            ),
                                            name
                                        );
                                        Repaint();
                                        enemySpawnDataReNameFloatingAreaHeader = default;
                                    },
                                    title: "Rename"
                                )
                            );
                            floatingArea.SetRect(enemySpawnDataReNameFloatingAreaHeader.GetAddY(-fileViewerScroll));
                        }
                    );*/
                    menu.AddItem(
                        new GUIContent("Copy"),
                        false,
                        () =>
                        {
                            EnemySpawnData spawnData = editorEnemyData.spawnData.Copy();
                            data.CreateEnemySpawnData(spawnData);

                            EditorEnemyData copy = new EditorEnemyData(
                                spawnData,
                                editorEnemyData.prefab,
                                editorEnemyData.prefabType
                            );
                            data.InsertToEditorEnemySpawnDataList(copy);
                            Repaint();
                            /*
                            string GetNextPath(string target)
                            {
                                string noExtensionPath = targetPath.Substring(0, targetPath.Length - ".asset".Length);

                                if (noExtensionPath[^1] == ')')
                                {
                                    string noNumberPath = noExtensionPath.Substring(
                                        0, 
                                        noExtensionPath.LastIndexOf('(')
                                    );
                                    //a (1).asset (11)
                                    //0123456789
                                    //noExtension: a (1) (5)
                                    //             01234
                                    //noNumber: a_ (2)
                                    //          01

                                    bool isNumber =
                                        int.TryParse(
                                            noExtensionPath.Substring(
                                                noNumberPath.Length + 1,
                                                noExtensionPath.Length - noNumberPath.Length - 2
                                            ),
                                            out int number
                                        );
                                    if (isNumber)
                                    {
                                        return $"{noNumberPath}({number + 1}).asset";
                                    }
                                    else return $"{noNumberPath}(_).asset";
                                }
                                else return $"{noExtensionPath}(Copy).asset";
                            }
                            */
                        }
                    );

                    menu.AddItem(
                        new GUIContent("Delete"),
                        false,
                        () =>
                        {
                            AssetDatabase.DeleteAsset(
                                AssetDatabase.GetAssetPath(
                                    editorEnemyData.spawnData
                                )
                            );
                            data.editorEnemySpawnDataList.Remove(editorEnemyData);
                            Repaint();
                        }
                    );

                    menu.ShowAsContext();
                }
            }
            
            void SelectEnemyData(int index)
            {
                GUI.FocusControl(null);
                if (data.selectedEnemyDataIndex != index)
                {
                    data.SelectEnemyData(index);
                }
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
                        CustomGUILayout.WarningLabel("Cannot Found Type: " + data.selectedEnemyData.spawnData.GetType());
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
                    else CustomGUILayout.WarningLabel("EditorGUI Missing");
                }
                else
                {
                    CustomGUILayout.WarningLabel("Select Enemy Spawn Data");
                }
            }
            GUILayout.EndArea();
        }
        
        void DrawPreview()
        {
            if (e.type == EventType.Repaint && previewRect.width > 0 && previewRect.height > 0)
            {
                _outputTexture = CreatePreviewTexture(previewRect);
            }
            if (_outputTexture != null)
                GUI.DrawTexture(previewRect, _outputTexture);

            Vector2Int cellCount = 3 * Vector2Int.one + new Vector2Int(
                Mathf.FloorToInt(previewRect.width / data.cellSize),
                Mathf.FloorToInt(previewRect.height / data.cellSize)
            );
            Vector2 offset = new Vector2((data.previewPos.x) % data.cellSize, (data.previewPos.y) % data.cellSize);
            Vector2 start = new Vector2(Mathf.Floor(previewRect.x / data.cellSize) * data.cellSize, Mathf.Floor(previewRect.y / data.cellSize) * data.cellSize);
            CustomGUI.DrawOpenGrid(start + offset - data.cellSize * Vector2.one, cellCount, data.cellSize, setting.previewOutGridColor);
            CustomGUI.DrawCloseGrid(data.previewPos + data.cellSize * new Vector2(-0.5f * Win.gameWidth, -Win.gameHeight), new Vector2Int(Win.gameWidth, Win.gameHeight), data.cellSize, setting.previewGameGridColor);

            if (data.selectedEnemyData != null)
            {
                DrawEnemyGizmos();
            }
            DrawTimeLine();
            
            void DrawEnemyGizmos()
            {
                foreach(EditorEnemyData editorEnemyData in data.sameTimeEnemyList)
                {
                    editorEnemyData.editorGUI.DrawSameTimeEnemyDataGizmos(editorEnemyData);
                }
                data.selectedEnemyData.editorGUI.DrawSelectedEnemyDataGizmos(data.selectedEnemyData);
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
                if (GameManager.instance.time != -1)
                {
                    Vector2 playTimeLineScreenPos = GetTimeScreenPos(GameManager.instance.time);
                    Handles.color = Color.green;
                    Handles.DrawLine(
                        playTimeLineScreenPos.AddY(setting.timeLengthFieldOffsetY),
                        playTimeLineScreenPos.AddY(-setting.timeLengthFieldOffsetY)
                    );
                    Handles.color = Color.white;
                }

                for (int i = 0; i < data.editorEnemySpawnDataList.Count; i++)
                {
                    if (i == data.selectedEnemyDataIndex) continue;

                    DrawTimeLineMarker(
                        data.editorEnemySpawnDataList[i].spawnData.spawnTime,
                        setting.enemySpawnTimeColor
                    );
                }
                if (data.selectedEnemyData != null)
                {
                    DrawTimeLineMarker(
                        data.selectedEnemyData.spawnData.spawnTime,
                        setting.selectEnemySpawnTimeColor
                    );
                }

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

                data.selectedStage.timeLength = EditorGUI.DelayedFloatField(timeLengthFieldRect, data.selectedStage.timeLength);
                if (data.editorEnemySpawnDataList.Count >= 1)
                {
                    data.selectedStage.timeLength = data.editorEnemySpawnDataList[^1].spawnData.spawnTime;
                }
            }
        }

        #endregion

        #region Utility

        void PreviewCheck()
        {
            Box previewBox = new Box(
                new Vector2(Win.gameWidth, Win.gameHeight) * data.cellSize,
                 new Vector2(0, -0.5f * Win.gameHeight * data.cellSize)
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
    }

    bool FileViewerWindowCheck(out float fileViewerLineMax)
    {
        fileViewerLineMax = position.width - (data.inspectorLinePosX + setting.minDistanceBetweenLines);

        return data.fileViewerLinePosX <= fileViewerLineMax;
    }
    bool InspectorWindowCheck(out float inspectorLineMax)
    {
        inspectorLineMax = position.width - (data.fileViewerLinePosX + setting.minDistanceBetweenLines);
        return data.inspectorLinePosX <= inspectorLineMax;
    }
    public static Vector2 WorldToScreenPos(Vector2 worldPos)
    {
        return data.previewPos + VectorUtility.MultiplyY(worldPos, -1) * data.cellSize;
    }
    public static Vector2 ScreenToWorldPoint(Vector2 screenPos)
    {
        return VectorUtility.MultiplyY(((screenPos - data.previewPos) / data.cellSize), -1);
    }
    public static Vector2 GetTimeScreenPos(float time)
    {
        float timeLineStart = data.fileViewerLinePosX + setting.timeHorizontalSpace;
        float timeLineEnd = (instance.position.width - data.inspectorLinePosX) - setting.timeHorizontalSpace;
        
        if (timeLineStart > timeLineEnd) return -Vector2.one;
        
        float timeLineY = instance.position.height - setting.timeBottomSpace;
        float timeRatio = time / data.selectedStage.timeLength;

        return new Vector2(Mathf.Lerp(timeLineStart, timeLineEnd, timeRatio), timeLineY);
    }
}
#endif