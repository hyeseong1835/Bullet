using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StageEditor : EditorWindow
{
    struct EditorEnemyData
    {
        public Vector2 screenPos;
        public Vector2 worldPos;

        public float spawnTime;

        public EditorEnemyData(Vector2 screenPos, Vector2 worldPos, float spawnTime)
        {
            this.screenPos = screenPos;
            this.worldPos = worldPos;

            this.spawnTime = spawnTime;
        }
    }
    List<EditorEnemyData> editorEnemyDataList = new List<EditorEnemyData>();

    EditorEnemyData selectedEnemyData = new EditorEnemyData(new Vector2(100, 300), Vector2.zero, 0);

    Vector2 center;

    float inspectorLinePosX = 100;
    bool holdInspectorLine = false;
    const float defaultInspectorWidth = 300;
    const float inspectorTopSpace = 10;
    const float inspectorRightSpace = 5;
    const float inspectorLeftSpace = 5;

    [MenuItem("Window/StageEditor")]
    static void CreateWindow()
    {
        StageEditor window = (StageEditor)EditorWindow.GetWindow(typeof(StageEditor));
        window.Show();
    }
    void OnGUI()
    {
        Event e = Event.current;
        float cellSize = 50;
        Vector2Int cellCount = new Vector2Int(Window.GameWidth + 4, Window.GameHeight + 4);
        float timeLength = 100;
        float timeCubeSize = 10;
        float timeHorizontalSpace = 100;
        float timeBottomSpace = 50;
        float screenMoveSpeed = 100;
        float inspectorLineHoldWidth = 10;

        Vector2 origin = center + Vector2.up * cellSize * (0.5f * Window.GameHeight);

        if (center.x < 0 || inspectorLinePosX < center.x || center.y < 0 || position.height < center.y)
        {
            Debug.Log($"Center({center.x}, {center.y}) is out of Window({position.size.x}, {position.size.y})");
            center = 0.5f * new Vector2(inspectorLinePosX, position.height);
            origin = center + Vector2.up * cellSize * (0.5f * Window.GameHeight);
            selectedEnemyData.screenPos = WorldToScreenPos(selectedEnemyData.worldPos);

        }
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
                center += move * screenMoveSpeed;
                origin = center + Vector2.up * cellSize * (0.5f * Window.GameHeight);
                selectedEnemyData.screenPos = WorldToScreenPos(selectedEnemyData.worldPos);

                e.Use();
                OnGUI();
            }
        }

        #endregion

        #region Grid

        Handles.color = Color.gray;
        for (int line = 1; line < cellCount.x; line++)
        {
            float x = center.x + cellSize * (0.5f * cellCount.x - line);
            float yHalf = 0.5f * cellSize * cellCount.y;

            Handles.DrawLine(new Vector3(x, center.y + yHalf, 0), new Vector3(x, center.y - yHalf, 0));
        }
        for (int line = 1; line < cellCount.y; line++)
        {
            float y = center.y + cellSize * (0.5f * cellCount.y - line);
            float xHalf = 0.5f * cellSize * cellCount.x;

            Handles.DrawLine(new Vector3(center.x + xHalf, y, 0), new Vector3(center.x - xHalf, y, 0));
        }

        Handles.color = Color.white;
        for (int line = 0; line < Window.GameWidth + 1; line++)
        {
            float x = center.x + cellSize * (0.5f * Window.GameWidth - line);
            float yHalf = 0.5f * cellSize * Window.GameHeight;

            Handles.DrawLine(new Vector3(x, center.y + yHalf, 0), new Vector3(x, center.y - yHalf, 0));
        }
        for (int line = 0; line < Window.GameHeight + 1; line++)
        {
            float y = center.y + cellSize * (0.5f * Window.GameHeight - line);
            float xHalf = 0.5f * cellSize * Window.GameWidth;

            Handles.DrawLine(new Vector3(center.x + xHalf, y, 0), new Vector3(center.x - xHalf, y, 0));
        }

        #endregion

        #region Enemy

        Handles.DrawSolidDisc(selectedEnemyData.screenPos, Vector3.forward, 10);

        float timeLineY = position.height - timeBottomSpace;
        Handles.DrawLine(
            new Vector3(timeHorizontalSpace, timeLineY), 
            new Vector3(inspectorLinePosX - timeHorizontalSpace, timeLineY)
        );
        Handles.DrawWireCube(
            new Vector3(timeHorizontalSpace + (selectedEnemyData.spawnTime / timeLength) * (inspectorLinePosX - 2 * timeHorizontalSpace), timeLineY), 
            Vector3.one * timeCubeSize
        );

        #endregion

        #region Inspector

        #region Area

        if (inspectorLinePosX < 0 ||position.width <= inspectorLinePosX)
        {
            Debug.Log($"InspectorLine({inspectorLinePosX}) is out of Window({0} ~ {position.width})");
            
            if (position.width > defaultInspectorWidth) inspectorLinePosX = defaultInspectorWidth;
            else inspectorLinePosX = 0.66f * position.width;
        }

        Handles.DrawSolidRectangleWithOutline(
            new Rect(inspectorLinePosX, 0, position.width - inspectorLinePosX, position.height),
            new Color(0.1f, 0.1f, 0.1f, 1f),
            Color.black
        );
        switch (e.type)
        {
            case EventType.MouseDown:
                float distance = e.mousePosition.x - inspectorLinePosX;
                if (-inspectorLineHoldWidth < distance && distance < inspectorLineHoldWidth)
                {
                    holdInspectorLine = true;
                    e.Use();
                }
                break;

            case EventType.MouseDrag:
                if (holdInspectorLine)
                {
                    inspectorLinePosX = e.mousePosition.x;
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

        #region Value

        GUILayout.BeginArea(new Rect(
            inspectorLinePosX + inspectorLeftSpace, inspectorTopSpace, 
            position.width - inspectorLinePosX - inspectorLeftSpace - inspectorRightSpace, position.height));
        
        GUILayout.Label("Enemy Inspector", EditorStyles.boldLabel);

        EditorGUILayout.Vector2Field("Screen Pos", selectedEnemyData.screenPos);
        Vector2 editedselectedEnemyDataWorldPos = EditorGUILayout.Vector2Field("World Pos", selectedEnemyData.worldPos);
        if (editedselectedEnemyDataWorldPos != selectedEnemyData.worldPos)
        {
            selectedEnemyData.worldPos = editedselectedEnemyDataWorldPos;
            selectedEnemyData.screenPos = WorldToScreenPos(editedselectedEnemyDataWorldPos);
        }
        
        selectedEnemyData.spawnTime = EditorGUILayout.FloatField("Spawn Time", selectedEnemyData.spawnTime);

        GUILayout.EndArea();

        #endregion

        #endregion

        #region Utility

        Vector2 ScreenToWorldPos(Vector2 screenPos)
        {
            return new Vector2(screenPos.x, -screenPos.y) / cellSize - origin;
        }
        Vector2 WorldToScreenPos(Vector2 worldPos)
        {
            return new Vector2(worldPos.x, - worldPos.y) * cellSize + origin;
        }

        #endregion
    }
}
