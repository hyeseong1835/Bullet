using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour
{
    public static GameCanvas instance;
    static CameraController cam => CameraController.instance;
    public RectTransform rect;
    [SerializeField] CanvasScaler canvasScaler;

    public float screenRatio => cam.screenRatio;

    public float height = 1;
    public float width = 1;
    
    public float scaleFactor = 1024;

    void Awake()
    {
        instance = this;
        rect = GetComponent<RectTransform>();
    }
    void Start()
    {
        cam.onScreenRatioChangeEvent.AddListener(OnChangeScreenRatio);
    }
    public void OnChangeScreenRatio()
    {
        float scale;
        if (cam.isDriveHeight)
        {
            scale = Screen.height / scaleFactor;
        }
        else
        {
            scale = Screen.width / scaleFactor * height / width;
        }
        canvasScaler.scaleFactor = scale;
        rect.anchoredPosition = new Vector2(0, 0.5f * height * cam.pixelPerUnit / scale);
        rect.sizeDelta = new Vector2(width * cam.pixelPerUnit / scale, height * cam.pixelPerUnit / scale);
    }
    void OnValidate()
    {
        instance = this;
    }
}
