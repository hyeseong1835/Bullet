using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "Stage", order = 0)]
public class Stage : ScriptableObject
{
    public Wave[] waves;
}
