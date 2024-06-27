using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterBox : MonoBehaviour
{
    static GameCanvas GameCanvas => GameCanvas.instance;

    public Transform top, bottom, right, left;
    [SerializeField] float size;

    public void Refresh()
    {
        if (GameCanvas == null) return;

        top.position = new Vector3(0, GameCanvas.height + 0.5f * size, 0);
        top.localScale = new Vector3(GameCanvas.width, size, 1);

        bottom.position = new Vector3(0, -0.5f * size, 0);
        bottom.localScale = new Vector3(GameCanvas.width, size, 1);

        right.position = new Vector3(0.5f * GameCanvas.width + 0.5f * size, 0.5f * GameCanvas.height, 0);
        right.localScale = new Vector3(size, GameCanvas.height + 2 * size, 1);

        left.position = new Vector3(-0.5f * GameCanvas.width - 0.5f * size, 0.5f * GameCanvas.height, 0);
        left.localScale = new Vector3(size, GameCanvas.height + 2 * size, 1);
    }
}
