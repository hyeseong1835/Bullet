using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public Camera cam;
    public float widthMagnify;
    void Awake()
    {
        instance = this;
        cam = Camera.main;
    }
    void Update()
    {
        if (EditorApplication.isPlaying == false)
        {
            if (instance == null) instance = this;
            if (cam == null) cam = Camera.main;
            {
                float ratio = (float)Screen.height / Screen.width;

                widthMagnify = ratio;
                cam.orthographicSize = widthMagnify;
                transform.position = new Vector3(0, ratio, -10);
            }
            
            return;
        }
    }
    void OnDrawGizmos()
    {

    }
}
