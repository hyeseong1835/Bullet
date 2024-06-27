using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour, IOnScreenResizedReceiver
{
    public static CameraController instance;
    public Camera cam;
  
    void Awake()
    {
        instance = this;
        if (cam == null) cam = GetComponent<Camera>();
    }

    void Update()
    {

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(new Vector2(0, 0.5f * Window.ScreenHeight), new Vector2(2, Window.ScreenHeight));
    }
    void OnValidate()
    {
        instance = this;
    }
    public void OnScreenResized()
    {
        Vector3 camPos = cam.transform.position;
        
        if (Window.isDriveHeight) camPos.y = 0.5f * Window.ScreenHeight;
        else camPos.y = 0.5f * Window.windowRatio * Window.ScreenWidth;

        cam.transform.position = camPos;
        cam.orthographicSize = camPos.y;
    }
}
