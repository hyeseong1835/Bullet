using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Setting", menuName = "StageEditor/Setting")]
public class StageEditorSetting : ScriptableObject
{
    public float lineHoldWidth = 10;
    public float minDistanceBetweenLines = 10;

    public float inspectorTopSpace = 10;
    public float inspectorBottomSpace = 5;
    public float inspectorRightSpace = 5;
    public float inspectorLeftSpace = 5;
    public SquareColor inspectorColor;
    public SquareColor dropDownColor;

    public float fileTopSpace = 10;
    public float fileBottomSpace = 5;
    public float fileRightSpace = 5;
    public float fileLeftSpace = 5;
    public SquareColor fileColor;
    public float timeWidth = 50;
    public Vector2 timeLengthFieldSize = new Vector2(50, 20);
    public float timeLengthFieldOffsetY = 5;
    public Color timeLineColor;
    public Color previewBackGroundColor;
    public Color previewGameGridColor;
    public Color previewOutGridColor;

    public float timeCubeSize = 10;
    public float timeHorizontalSpace = 100;
    public float timeBottomSpace = 50;
    public SquareColor selectEnemySpawnTimeColor;
    public SquareColor enemySpawnTimeColor;

    public float screenMoveSpeed = 50;
    public float zoomSpeed = 10;
    public float cellSizeMin = 5;

    public float definitionGizmoSize = 10;

    public float buttonHeight = 20;
    public float buttonWidth = 100;

    public SquareColor selectBoxColor;
    public Color selectHideInHeaderColor;
}
