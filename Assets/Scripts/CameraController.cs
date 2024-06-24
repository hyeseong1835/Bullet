using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public Camera screenCam;
    public Camera gameCam;
    public Canvas canvas;
    public RectTransform sceneRectTransform;
    
    public float widthMagnify;
    public float height;

    void Awake()
    {
        instance = this;
    }
    void Update()
    {
        if (EditorApplication.isPlaying)
        {
            
            
            return;
        }
        else
        {
            if (instance == null) instance = this;

            float ratio = (float)Screen.height / Screen.width;

            widthMagnify = ratio;
            height = 2 * ratio;
            screenCam.orthographicSize = widthMagnify;
            transform.position = new Vector3(0, ratio, -10);
        }
    }
    void OnDrawGizmos()
    {

    }
}
