using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterBox : MonoBehaviour
{
    static CameraController cam => CameraController.instance;
    public Transform top, bottom, right, left;
    [SerializeField] float size;

    public void Refresh()
    {
        top.position = new Vector3(0, cam.height + 0.5f * size, 0);
        top.localScale = new Vector3(cam.width, size, 1);

        bottom.position = new Vector3(0, -0.5f * size, 0);
        bottom.localScale = new Vector3(cam.width, size, 1);

        right.position = new Vector3(0.5f * cam.width + 0.5f * size, 0.5f * cam.height, 0);
        right.localScale = new Vector3(size, cam.height + 2 * size, 1);

        left.position = new Vector3(-0.5f * cam.width - 0.5f * size, 0.5f * cam.height, 0);
        left.localScale = new Vector3(size, cam.height + 2 * size, 1);
    }
}
