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

    public float fileTopSpace = 10;
    public float fileBottomSpace = 5;
    public float fileRightSpace = 5;
    public float fileLeftSpace = 5;

    public float timeCubeSize = 10;
    public float timeHorizontalSpace = 100;
    public float timeBottomSpace = 50;
    public Color selectEnemySpawnTimeFaceColor = new Color(1, 0.3f, 0.3f, 1);
    public Color selectEnemySpawnTimeOutlineColor = Color.white;

    public float screenMoveSpeed = 50;

    public float definitionGizmoSize = 10;

    public float buttonHeight = 20;
    public float buttonWidth = 100;
    public Color selectBoxFaceColor = new Color(1, 1, 1, 0.1f);
    public Color selectBoxOutlineColor = Color.white;

}
