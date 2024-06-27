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
    [SerializeField] LetterBox letterBox;

    public float screenRatio;
    public float pixelPerUnit;
    float prevScreenRatio;

    public UnityEvent onScreenRatioChangeEvent;

    public bool isDriveHeight;

    void Awake()
    {
        instance = this;
        cam = GetComponent<Camera>();

        Refresh();
    }
    void Update()
    {
        screenRatio = (float)Window.Height / Window.Width;

        if (screenRatio != prevScreenRatio)
        {
            Refresh();
            onScreenRatioChangeEvent.Invoke();
        }

        prevScreenRatio = screenRatio;

#if UNITY_EDITOR

        instance = this;
        Refresh();

#endif
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
        letterBox.Refresh();
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(new Vector2(0, 0.5f * Screen.screenHeight), new Vector2(2, Screen.screenHeight));
    }
}
