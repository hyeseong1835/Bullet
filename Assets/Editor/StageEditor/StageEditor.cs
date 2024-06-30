using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEngine;

public class StageEditor : EditorWindow
{
    static StageEditorData data;
    static StageEditorSetting setting;

    bool holdInspectorLine = false;

    Vector2Int cellCount;
    static float timeLength = 100;
    const float inspectorLineHoldWidth = 10;


    [MenuItem("Window/StageEditor")]
    static void CreateWindow()
    {
        StageEditor window = (StageEditor)EditorWindow.GetWindow(typeof(StageEditor));
        window.Show();
    }
    void OnGUI()
    {
        Event e = Event.current;
        if (data == null) data = (StageEditorData)EditorResources.Load<ScriptableObject>("StageEditor/Data.asset");
        if (setting == null) setting = (StageEditorSetting)EditorResources.Load<ScriptableObject>("StageEditor/Setting.asset");
        
        cellCount = new Vector2Int(Window.GameWidth + 4, Window.GameHeight + 4);

        if (data.preview.IsContact(data.previewPos, position.height, 0, data.inspectorLinePosX, 0, out Vector2 previewContact))
        {
            data.previewPos = previewContact;
        }
        Handles.DrawSolidDisc(data.previewPos, Vector3.forward, 10);

        #region Screen Move

        Vector2 move;

        if (e.type == EventType.KeyDown)
        {
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

                e.Use();
                OnGUI();
            }
        }

        #endregion

        #region Grid
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

        #endregion


        #region Inspector

        #region Area

        if (data.inspectorLinePosX < 0 ||position.width <= data.inspectorLinePosX)
        {
            if (position.width > setting.defaultInspectorWidth) data.inspectorLinePosX = setting.defaultInspectorWidth;
            else data.inspectorLinePosX = 0.66f * position.width;
        }

        Handles.DrawSolidRectangleWithOutline(
            new Rect(data.inspectorLinePosX, 0, position.width - data.inspectorLinePosX, position.height),
            new Color(0.1f, 0.1f, 0.1f, 1f),
            Color.black
        );
        switch (e.type)
        {
            case EventType.MouseDown:
                float distance = e.mousePosition.x - data.inspectorLinePosX;
                if (-inspectorLineHoldWidth < distance && distance < inspectorLineHoldWidth)
                {
                    holdInspectorLine = true;
                    e.Use();
                }
                break;

            case EventType.MouseDrag:
                if (holdInspectorLine)
                {
                    data.inspectorLinePosX = e.mousePosition.x;
                    e.Use();
                    OnGUI();
                }
                break;
            case EventType.MouseUp:
                if (holdInspectorLine)
                {
                    holdInspectorLine = false;
                }
                break;
        }
        #endregion

        DrawEnemyDataInInspector();
        //DrawEnemyDataGizmos();

        #endregion
    }
    void DrawEnemyDataInInspector()
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


        float buttonHeight = 20;
        float buttonWidth = 50;

        Rect applyButtonRect = new Rect();
        applyButtonRect.x = position.width - (buttonWidth + setting.inspectorRightSpace);
        applyButtonRect.y = position.height - 10 - 0.5f * buttonHeight - setting.inspectorBottomSpace;
        applyButtonRect.width = buttonWidth;
        applyButtonRect.height = buttonHeight;

        Rect loadButtonRect = new Rect();
        loadButtonRect.x = data.inspectorLinePosX + (setting.inspectorLeftSpace);
        loadButtonRect.y = position.height - 10 - 0.5f * buttonHeight - setting.inspectorBottomSpace;
        loadButtonRect.width = buttonWidth;
        loadButtonRect.height = buttonHeight;
        if (GUI.Button(applyButtonRect, "Apply"))
        {

        }
        if (GUI.Button(loadButtonRect, "Load"))
        {

        }
    }
    void DrawEnemyDataGizmos()
    {
        Handles.DrawSolidDisc(data.selectedEnemy.worldPos, Vector3.forward, 10);

        float timeLineY = position.height - setting.timeBottomSpace;
        Handles.DrawLine(
            new Vector3(setting.timeHorizontalSpace, timeLineY),
            new Vector3(data.inspectorLinePosX - setting.timeHorizontalSpace, timeLineY)
        );
        Handles.DrawWireCube(
            new Vector3(setting.timeHorizontalSpace + (data.selectedEnemy.spawnTime / timeLength) * (data.inspectorLinePosX - 2 * setting.timeHorizontalSpace), timeLineY),
            Vector3.one * setting.timeCubeSize
        );
        Handles.color = Color.cyan;
        Vector2 definitionScreenPos = WorldToScreenPos(data.selectedEnemy.definition);

        Handles.DrawLine(WorldToScreenPos(data.selectedEnemy.worldPos), definitionScreenPos);
        Vector2 X = new Vector2(setting.definitionGizmoSize, -setting.definitionGizmoSize);
        Handles.DrawLine(definitionScreenPos + X, definitionScreenPos - X);
        Handles.DrawLine(definitionScreenPos + Vector2.one * setting.definitionGizmoSize, definitionScreenPos - Vector2.one * setting.definitionGizmoSize);

    }

    public static Vector2 ScreenToWorldPos(Vector2 screenPos)
    {
        return new Vector2(screenPos.x, -screenPos.y) / data.cellSize - (data.previewPos + Vector2.up * data.cellSize * (0.5f * Window.GameHeight));
    }
    public static Vector2 WorldToScreenPos(Vector2 worldPos)
    {
        return new Vector2(worldPos.x, -worldPos.y) * data.cellSize + data.previewPos + Vector2.up * data.cellSize * (0.5f * Window.GameHeight);
    }
}
