using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour, IOnScreenResizedReceiver
{
    public static CameraController instance;
    static Window Win => Window.instance;

    public Camera cam;
    public Vector2 viewSize;
  
    void Awake()
    {
        instance = this;
        if (cam == null) cam = GetComponent<Camera>();
    }

    void OnEnable()
    {
        Win.onScreenResizedRecieverList.Add(this);
    }
    void OnDisable()
    {
        Win.onScreenResizedRecieverList.Remove(this);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(new Vector2(0, 0.5f * Win.screenHeight), new Vector2(2, Win.screenHeight));
    }
    void OnValidate()
    {
        instance = this;
    }
    public void OnScreenResized()
    {
        if (Win.isDriveHeight)
        {
            viewSize = new Vector2(Win.screenHeight / Win.windowRatio, Win.screenHeight);
        }
        else viewSize = new Vector2(Win.screenWidth, Win.screenWidth * Win.windowRatio);
        cam.transform.position = cam.transform.position.GetSetY(0.5f * viewSize.y);
        cam.orthographicSize = 0.5f * viewSize.y;
    }
}
