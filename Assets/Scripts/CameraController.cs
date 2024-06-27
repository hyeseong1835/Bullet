using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
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
}
