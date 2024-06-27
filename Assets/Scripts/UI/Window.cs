using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteAlways]
public class Window : MonoBehaviour
{
    public static Window instance;
    static CameraController cam => CameraController.instance;
    public Canvas windowCanvas;
    public RectTransform screenRect;
    public RectTransform gameRect;
    
    public CanvasScaler canvasScaler;


    public static int WindowHeight => Screen.height;
    public static int WindowWidth => Screen.width;
    public static float windowRatio { get; private set; }

    public static float ScreenHeight => instance.screenHeight;
    [SerializeField] float screenHeight = 1;
    public static float ScreenWidth => instance.screenWidth;
    [SerializeField] float screenWidth = 1;
    public static float screenRatio { get; private set; }
    public static float screenUp { get; private set; }
    public static float screenDown { get; private set; }
    public static float screenRight { get; private set; }
    public static float screenLeft { get; private set; }

    public static float GameHeight => instance.gameHeight;
    [SerializeField] float gameHeight = 1;
    public static float GameWidth => instance.gameWidth;
    [SerializeField] float gameWidth = 1;
    public static float gameRatio { get; private set; }
    public static float gameUp { get; private set; }
    public static float gameDown { get; private set; }
    public static float gameRight { get; private set; }
    public static float gameLeft { get; private set; }


    public float scaleFactor = 1024;

    public float pixelPerUnit { get; private set; }

    public bool isDriveHeight { get; private set; }

    public Action onScreenResized;

    float prevWindowHeight;
    float prevWindowWidth;

    void Set()
    {
        instance = this;

        screenRatio = screenWidth / screenHeight;
        screenUp = screenHeight;
        screenDown = 0;
        screenRight = 0.5f * screenWidth;
        screenLeft = -0.5f * screenWidth;

        gameRatio = gameWidth / gameHeight;
        gameUp = gameHeight;
        gameDown = 0;
        gameRight = 0.5f * gameWidth;
        gameLeft = -0.5f * gameWidth;
    }
    void Awake()
    {
        Set();
    }
    void Update()
    {
        if (WindowHeight != prevWindowHeight || WindowWidth != prevWindowWidth)
        {
            windowRatio = (float)WindowHeight / WindowWidth;
            
            Refresh();

            if (onScreenResized.IsUnityNull() == false) onScreenResized.Invoke();

            prevWindowHeight = WindowHeight;
            prevWindowWidth = WindowWidth;
        }
    }
    public void Refresh()
    {
        isDriveHeight = windowRatio < screenRatio;

        pixelPerUnit = isDriveHeight ? (WindowHeight / screenHeight) : (WindowWidth / screenWidth);
        
        Vector2 camPos = cam.transform.position;
        float scale;
        if (isDriveHeight)
        {
            camPos.y = 0.5f * screenHeight;

            scale = WindowHeight / scaleFactor;
        }
        else
        {
            camPos.y = 0.5f * windowRatio * screenWidth;

            scale = WindowWidth * screenRatio / scaleFactor;
        }
        cam.transform.position = camPos;
        cam.cam.orthographicSize = camPos.y;

        canvasScaler.scaleFactor = scale;

        float factor = pixelPerUnit / scale;

        screenRect.anchoredPosition = new Vector2(0, 0.5f * screenHeight) * factor;
        screenRect.sizeDelta = new Vector2(screenWidth, screenHeight) * factor;

        gameRect.anchoredPosition = new Vector2(0, 0.5f * gameHeight) * factor;
        gameRect.sizeDelta = new Vector2(gameWidth , gameHeight) * factor;
    }
    void OnValidate()
    {
        Set();
    }
}