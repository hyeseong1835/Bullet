using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Setting", menuName = "StageEditor/Setting")]
public class StageEditorSetting : ScriptableObject
{
    public float inspectorTopSpace = 10;
    public float inspectorBottomSpace = 5;
    public float inspectorRightSpace = 5;
    public float inspectorLeftSpace = 5;
    public float defaultInspectorWidth = 300;

    public float timeCubeSize = 10;
    public float timeHorizontalSpace = 100;
    public float timeBottomSpace = 50;
    public float screenMoveSpeed = 50;

    public float definitionGizmoSize = 10;

}
