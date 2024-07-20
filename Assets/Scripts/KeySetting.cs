using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Key Setting", menuName = "Key Setting")]
public class KeySetting : ScriptableObject
{
    public KeyCode moveUp = KeyCode.W;
    public KeyCode moveDown = KeyCode.S;
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;

    public KeyCode attack = KeyCode.Mouse0;
    public KeyCode skill = KeyCode.Mouse1;

    public KeyCode weaponBreak = KeyCode.LeftShift;
}
