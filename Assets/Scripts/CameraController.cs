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

    public float height = 5;
    public float width = 2;

    public float screenRatio;
    public float pixelPerUnit;
    float prevScreenRatio;

    public UnityEvent onScreenRatioChangeEvent;

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
        if (screenRatio < height / width)
        {
            cam.orthographicSize = 0.5f * height;
            transform.position = new Vector3(transform.position.x, 0.5f * height, transform.position.z);
            pixelPerUnit = Screen.height / height;
        }
        else
        {
            cam.orthographicSize = 0.5f * screenRatio * width;
            transform.position = new Vector3(transform.position.x, 0.5f * screenRatio * width, transform.position.z);
            pixelPerUnit = Screen.width / width;
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
        Gizmos.DrawWireCube(new Vector2(0, 0.5f * height), new Vector2(2, height));
    }
}
