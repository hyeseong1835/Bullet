using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEditorInternal.Profiling.Memory.Experimental;
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

        if (e.isScrollWheel && previewRect.Contains(e.mousePosition))
        {
            switch(e.pointerType)
            {
                case PointerType.Mouse:
                    data.cellSize += e.delta.y;
                    break;
            }
            if (data.cellSize < setting.cellSizeMin) data.cellSize = setting.cellSizeMin;
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
            GUI.FocusControl(null);
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
            {
                GUILayout.Label("File Viewer", EditorStyles.boldLabel);
                
                //Stage Select
                GUILayout.BeginHorizontal();
                {
                    int stageSelectInput = EditorGUILayout.Popup(data.selectedStageIndex, data.stageNameArray, GUILayout.Height(setting.buttonHeight));
                    if (stageSelectInput != data.selectedStageIndex)
                    {
                        string stageFolderPath = $"Stage/{data.stageNameArray[stageSelectInput]}";

                        data.selectedStage = (Stage)Resources.Load<ScriptableObject>($"{stageFolderPath}/Stage Data");
                        data.RefreshEnemyDataList();

                        int enemySelectIndex;
                        if (data.enemySpawnDataList.Count > 0) enemySelectIndex = 0;
                        else enemySelectIndex = -1;
                        data.SelectEnemySpawnData(enemySelectIndex);

                        data.selectedStageIndex = stageSelectInput;
                    }

                    //Refresh
                    if (GUI.Button(GUILayoutUtility.GetRect(setting.buttonWidth, setting.buttonHeight).GetAddY(2), "Refresh"))
                    {
                        data.stageNameArray = Directory.GetDirectories("Assets/Resources/Stage")
                            .Select(path =>
                            {
                                string splitPath = path.Split('/')[^1];
                                return splitPath.Substring(6, splitPath.Length - 6);
                            }
                        ).ToArray();

                        if (data.selectedStage == null) data.enemySpawnDataList.Clear();
                        else data.RefreshEnemyDataList();
                        float prevTime = -1;
                        Dictionary<float, bool> timeFoldout = new Dictionary<float, bool>();
                        foreach (EnemySpawnData enemyData in data.enemySpawnDataList)
                        {
                            if (enemyData.spawnTime != prevTime)
                            {
                                if (data.timeFoldout.TryGetValue(enemyData.spawnTime, out bool foldout))
                                {
                                    timeFoldout.Add(enemyData.spawnTime, foldout);
                                }
                                else
                                {
                                    timeFoldout.Add(enemyData.spawnTime, false);
                                }
                            }
                        }
                        data.timeFoldout = timeFoldout;
                    }
                }
                GUILayout.EndHorizontal();
                
                EditorGUILayout.Space(5);

                if (data.enemySpawnDataList == null || data.enemySpawnDataList.Count < 1)
                {
                    GUILayout.Label("EnemySpawnData is Empty");
                }
                else
                {
                    // Enemy List
                    data.enemyScroll = EditorGUILayout.BeginScrollView(new Vector2(0, data.enemyScroll)).y;
                    {
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
                                if(foldout == false) LateCloseHeader();
                                
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
                    EditorGUILayout.EndScrollView();
                }
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
            {
                GUILayout.Label("Enemy Inspector", EditorStyles.boldLabel);

                if (data.selectedEnemySpawnData != null)
                {
                    #region SpawnPrefab
                    data.selectedEnemySpawnData.prefabIndex = EditorGUILayout.Popup(data.selectedEnemySpawnData.prefabIndex, data.prefabs.Select(prefab => prefab.name).ToArray());
                    #endregion

                    #region SpawnTime
                    float spawnTimeInput = EditorGUILayout.FloatField("Spawn Time", data.selectedEnemySpawnData.spawnTime);
                    if (spawnTimeInput != data.selectedEnemySpawnData.spawnTime)
                    {
                        if (spawnTimeInput >= 0) data.selectedEnemySpawnData.spawnTime = spawnTimeInput;
                        else data.selectedEnemySpawnData.spawnTime = 0;

                        data.SelectEnemySpawnData(data.RemoveAndSortInEnemySpawnDataList(data.selectedEnemySpawnData, data.selectedEnemyIndex));
                    }
                    #endregion

                    if (data.selectedEnemyEditorGUI == null) data.RefreshEnemyEditorGUI();
                    
                    if (data.selectedEnemyEditorGUI != null) data.selectedEnemyEditorGUI.DrawInspectorGUI(data.selectedEnemySpawnData);
                    else GUILayout.Label("EditorGUI Missing");
                }
                else
                {
                    GUILayout.Label("Select Enemy Spawn Data");
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
            float timeLineX = data.filesLinePosX + setting.timeHorizontalSpace;
            float timeLineY = position.height - setting.timeBottomSpace;
            float timeLineWidth = data.inspectorLinePosX - 2 * setting.timeHorizontalSpace;

            float timeLineStart = data.filesLinePosX + setting.timeHorizontalSpace;
            float timeLineEnd = data.inspectorLinePosX - setting.timeHorizontalSpace;

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
            if (data.selectedEnemyIndex != -1)
            {
                DrawTimeLineMarker(
                    data.enemySpawnDataList[data.selectedEnemyIndex].spawnTime,
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
