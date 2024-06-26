using UnityEditor;
using UnityEngine;
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

    void Awake()
    {
        instance = this;
        cam = GetComponent<Camera>();

        Refresh();
    }
    void Update()
    {
#if UNITY_EDITOR

        instance = this;
        Refresh();

#endif
    }
    void Refresh()
    {
        float ratio = (float)Screen.height / Screen.width;
        
        if (ratio < height / width)
        {
            cam.orthographicSize = 0.5f * height;
            transform.position = new Vector3(transform.position.x, cam.orthographicSize, transform.position.z);
        }
        else//ratio / width == 0.5
        {
            cam.orthographicSize = 0.5f * ratio * width;
            transform.position = new Vector3(transform.position.x, cam.orthographicSize, transform.position.z);
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
