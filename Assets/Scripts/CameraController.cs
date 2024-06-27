using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public Camera cam;
    [SerializeField] LetterBox letterBox;
    static GameCanvas GameCanvas => GameCanvas.instance;

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
        screenRatio = (float)Screen.height / Screen.width;

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
        if (screenRatio < GameCanvas.height / GameCanvas.width)
        {
            cam.orthographicSize = 0.5f * GameCanvas.height;
            transform.position = new Vector3(transform.position.x, 0.5f * GameCanvas.height, transform.position.z);
            pixelPerUnit = Screen.height / GameCanvas.height;
            isDriveHeight = true;
        }
        else
        {
            cam.orthographicSize = 0.5f * screenRatio * GameCanvas.width;
            transform.position = new Vector3(transform.position.x, 0.5f * screenRatio * GameCanvas.width, transform.position.z);
            pixelPerUnit = Screen.width / GameCanvas.width;
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
        Gizmos.DrawWireCube(new Vector2(0, 0.5f * GameCanvas.height), new Vector2(2, GameCanvas.height));
    }
}
