using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public Camera cam;
    
    public float height = 5;
    public float width = 2;

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

            if (ratio < height / width)
            {
                cam.orthographicSize = 0.5f * height;
                transform.position = new Vector3(0, cam.orthographicSize, -10);
            }
            else
            {
                cam.orthographicSize = ratio / (0.5f * width);
                transform.position = new Vector3(0, ratio / (0.5f * width), -10);
            }
        }
        #endif
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(new Vector2(0, 0.5f * height), new Vector2(2, height));
    }
}
