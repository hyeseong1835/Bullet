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

    public Camera cam;
    
    public float widthMagnify;
    public float height;

    void Awake()
    {
        instance = this;
    }
    void Update()
    {
        #if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
        #endif



        #if UNITY_EDITOR
        }
        else
        {
            if (instance == null) instance = this;
            if (cam == null) cam = GetComponent<Camera>();

            float ratio = (float)Screen.height / Screen.width;

            height = 2 * ratio / widthMagnify;
            cam.orthographicSize = ratio / widthMagnify;
            transform.position = new Vector3(0, ratio / widthMagnify, -10);
        }
        #endif
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(new Vector2(0, 0.5f * height), new Vector2(2, height));
    }
}
