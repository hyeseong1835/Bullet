using UnityEditor;
using UnityEngine;

public class InstantEnemyEditorGUI : EnemyEditorGUI
{
    static Event e => UnityEngine.Event.current;
    static Texture2D prefabPreview;
    public override void Event()
    {
        switch (e.type)
        {
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
        }
        void Mouse0Down()
        {

        }
        void Mouse0Drag()
        {

        }
        void Mouse0Up()
        {

        }
    }
    
    public override void DrawInspectorGUI(EditorEnemyData enemyData)
    {
        InstantEnemySpawnData data = (InstantEnemySpawnData)enemyData.spawnData;

        data.startPos = EditorGUILayout.Vector2Field("World Pos", data.startPos);
        data.endPos = EditorGUILayout.Vector2Field("Definition", data.endPos);
    }

    public override void DrawEnemyDataGizmos(EditorEnemyData enemyData)
    {
        InstantEnemySpawnData data = (InstantEnemySpawnData)enemyData.spawnData;

        if (false && prefabPreview == null)
        {
            var previewRender = new PreviewRenderUtility();
            previewRender.camera.backgroundColor = Color.clear;
            previewRender.camera.clearFlags = CameraClearFlags.Color;
            previewRender.camera.cameraType = CameraType.Game;
            previewRender.camera.farClipPlane = 1000f;
            previewRender.camera.nearClipPlane = 0.1f;

            previewRender.BeginStaticPreview(new Rect(0.0f, 0.0f, 1080.0f / 4.0f, 1920.0f / 4.0f));

            var canvasInstance = previewRender.InstantiatePrefabInScene(enemyData.prefab).GetComponent<Canvas>();
            canvasInstance.renderMode = RenderMode.ScreenSpaceCamera;
            canvasInstance.worldCamera = previewRender.camera;

            previewRender.Render();

            prefabPreview = previewRender.EndStaticPreview();

            previewRender.camera.targetTexture = null;
            previewRender.Cleanup();
        }
        Vector2 startScreenPos = StageEditor.WorldToScreenPos(data.startPos);
        Vector2 endScreenPos = StageEditor.WorldToScreenPos(data.endPos);

        GUI.DrawTexture(new Rect(startScreenPos, 50 * Vector2.one), prefabPreview ?? Texture2D.whiteTexture);
        
        Handles.color = Color.cyan;
        Handles.DrawLine(startScreenPos, endScreenPos);
        Vector2 X = new Vector2(StageEditor.setting.definitionGizmoSize, -StageEditor.setting.definitionGizmoSize);
        Handles.DrawLine(endScreenPos + X, endScreenPos - X);
        Handles.DrawLine(endScreenPos + Vector2.one * StageEditor.setting.definitionGizmoSize, endScreenPos - Vector2.one * StageEditor.setting.definitionGizmoSize);
        Handles.color = Color.white;
    }
}
