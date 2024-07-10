using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour, IOnScreenResizedReceiver
{
    public static CameraController instance;
    public Camera cam;
    public Vector2 viewSize;
  
    void Awake()
    {
        instance = this;
        if (cam == null) cam = GetComponent<Camera>();
    }

    void OnEnable()
    {
        Window.onScreenResizedRecieverList.Add(this);
    }
    void OnDisable()
    {
        Window.onScreenResizedRecieverList.Remove(this);
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
        if (Window.isDriveHeight)
        {
            viewSize = new Vector2(Window.ScreenHeight / Window.windowRatio, Window.ScreenHeight);
        }
        else viewSize = new Vector2(Window.ScreenWidth, Window.ScreenWidth * Window.windowRatio);
        cam.transform.position = cam.transform.position.GetSetY(0.5f * viewSize.y);
        cam.orthographicSize = 0.5f * viewSize.y;
    }
}
