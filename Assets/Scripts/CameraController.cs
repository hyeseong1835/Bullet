using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
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
        Win.onScreenResized += OnScreenResized;
    }
    void OnDisable()
    {
        Win.onScreenResized -= OnScreenResized;
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
        cam.transform.position = VectorUtility.SetY(cam.transform.position, 0.5f * viewSize.y);
        cam.orthographicSize = 0.5f * viewSize.y;
    }
}
