using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "StageEditor/Data")]
public class StageEditorData : ScriptableObject
{
    public struct EditorEnemyData
    {
        public Vector2 worldPos;
        public Vector2 definition;
        public float spawnTime;

        public EditorEnemyData(Vector2 worldPos, Vector2 definition, float spawnTime)
        {
            this.worldPos = worldPos;

            this.definition = definition;

            this.spawnTime = spawnTime;
        }
    }
    public Box preview;
    public Vector2 previewPos;

    public List<EnemySpawnData> editorEnemyDataList = new List<EnemySpawnData>();

    public EnemySpawnData selectedEnemy;

    public float cellSize = 50;

    public float inspectorLinePosX;
    public float filesLinePosX;

    private void OnValidate()
    {
        preview = new Box();
        preview.size = new Vector2(Window.GameWidth, Window.GameHeight) * cellSize;
        preview.center = new Vector2(0, -0.5f * Window.GameHeight * cellSize);
    }
}
