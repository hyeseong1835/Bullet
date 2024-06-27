using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class Window : MonoBehaviour
{
    #region 정적 필드

    public static Window instance;

    public static int Height => Screen.height;
    public static int Width => Screen.width;

    public static float ScreenUp => instance.screenHeight;
    public static float ScreenDown => 0;
    public static float ScreenRight => 0.5f * instance.screenWidth;
    public static float ScreenLeft => -0.5f * instance.screenWidth;

    public static float GameUp => instance.gameHeight;
    public static float GameDown => 0;
    public static float GameRight => 0.5f * instance.gameWidth;
    public static float GameLeft => -0.5f * instance.gameWidth;

    #endregion

    static CameraController cam => CameraController.instance;

    public Canvas windowCanvas;
    public RectTransform screenRect;
    public RectTransform gameRect;
    
    public CanvasScaler canvasScaler;


    public float screenHeight = 1;
    public float screenWidth = 1;

    public float gameHeight = 1;
    public float gameWidth = 1;
    
    public float scaleFactor = 1024;

    public delegate void OnScreenResized();
    public OnScreenResized onScreenResized;

    float prevWindowHeight;
    float prevWindowWidth;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {

    }
    void Update()
    {

    }
    private void LateUpdate()
    {
        if (Window.Height != prevWindowHeight || Window.Width != prevWindowWidth)
        {
            Refresh();
            Debug.Log("Screen Resized");
            onScreenResized.Invoke();
        }

        prevWindowHeight = Window.Height;
        prevWindowWidth = Window.Width;
    }
    public void Refresh()
    {
        float scale;
        if (cam.isDriveHeight)
        {
            scale = Height / scaleFactor;
        }
        else
        {
            scale = Width / scaleFactor * screenHeight / screenWidth;
        }
        canvasScaler.scaleFactor = scale;

        screenRect.anchoredPosition = new Vector2(0, 0.5f * screenHeight * cam.pixelPerUnit / scale);
        screenRect.sizeDelta = new Vector2(screenWidth * cam.pixelPerUnit / scale, screenHeight * cam.pixelPerUnit / scale);
        
        gameRect.anchoredPosition = new Vector2(0, 0.5f * gameHeight * cam.pixelPerUnit / scale);
        gameRect.sizeDelta = new Vector2(gameWidth * cam.pixelPerUnit / scale, gameHeight * cam.pixelPerUnit / scale);
    }
    void OnValidate()
    {
        instance = this;
        if (canvasScaler == null) canvasScaler = GetComponent<CanvasScaler>();
        if (windowCanvas == null) windowCanvas = GetComponent<Canvas>();
        if (screenRect == null) screenRect = transform.Find("Screen Canvas").GetComponent<RectTransform>();
        if (gameRect == null) gameRect = transform.Find("Game Canvas").GetComponent<RectTransform>();
    }
}
