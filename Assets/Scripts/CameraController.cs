using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    static Window Screen => Window.instance;
    public Camera cam;

    public float screenRatio;
    public float pixelPerUnit;

    public bool isDriveHeight;


    void Awake()
    {
        instance = this;
        if (cam == null) cam = GetComponent<Camera>();

        Refresh();
    }
    void Update()
    {
        screenRatio = (float)Window.Height / Window.Width;
        Screen.onScreenResized += Refresh;
    }

    void Refresh()
    {
        if (screenRatio < Screen.screenHeight / Screen.screenWidth)
        {
            cam.orthographicSize = 0.5f * Screen.screenHeight;
            transform.position = new Vector3(transform.position.x, 0.5f * Screen.screenHeight, transform.position.z);
            pixelPerUnit = Window.Height / Screen.screenHeight;
            isDriveHeight = true;
        }
        else
        {
            cam.orthographicSize = 0.5f * screenRatio * Screen.screenWidth;
            transform.position = new Vector3(transform.position.x, 0.5f * screenRatio * Screen.screenWidth, transform.position.z);
            pixelPerUnit = Window.Width / Screen.screenWidth;
            isDriveHeight = false;
        }
    }
    void OnValidate()
    {
        instance = this;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(new Vector2(0, 0.5f * Screen.screenHeight), new Vector2(2, Screen.screenHeight));
    }
}
