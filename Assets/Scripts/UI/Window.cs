using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class Window : MonoBehaviour
{
    public static Window instance;
    public Canvas windowCanvas { get; private set; }
    public CanvasScaler canvasScaler { get; private set; }
    public RectTransform screenRect { get; private set; }
    public RectTransform gameRect { get; private set; }


    public static int WindowHeight => Screen.height;
    public static int WindowWidth => Screen.width;
    /// <summary>
    /// Height / Width
    /// </summary>
    public float windowRatio { get; private set; }

    public Vector2 ScreenSize => new Vector2(screenWidth, screenHeight);
    public Vector2Int GameSize => new Vector2Int(gameWidth, gameHeight);

    public float scaleFactor = 1024;
    public float pixelPerUnit { get; private set; }
    public bool isDriveHeight { get; private set; }
    
    [Header("Screen")]
    public float screenHeight = 5;
    public float screenWidth = 4;
    public float screenRatio { get; private set; }
    public float screenUp { get; private set; }
    public float screenDown { get; private set; }
    public float screenRight { get; private set; }
    public float screenLeft { get; private set; }

    [Header("Game")]
    public int gameHeight = 5;
    public int gameWidth = 4;
    public float gameRatio { get; private set; }
    public float gameTop { get; private set; }
    public float gameBottom { get; private set; }
    public float gameRight { get; private set; }
    public float gameLeft { get; private set; }

    [NonSerialized] public Action onScreenResized;
#if UNITY_EDITOR
    [NonSerialized] public Action onWindowValidate;
#endif

    float prevWindowHeight;
    float prevWindowWidth;

    void Set()
    {
        instance = this;

        if (windowCanvas == null) windowCanvas = GetComponent<Canvas>();
        if (canvasScaler == null) canvasScaler = GetComponent<CanvasScaler>();
        if (screenRect == null) screenRect = transform.Find("Screen Canvas").GetComponent<RectTransform>();
        if (gameRect == null) gameRect = transform.Find("Game Canvas").GetComponent<RectTransform>();

        screenRatio = screenHeight / screenWidth;
        screenUp = screenHeight;
        screenDown = 0;
        screenRight = 0.5f * screenWidth;
        screenLeft = -0.5f * screenWidth;

        gameRatio = (float)gameHeight / gameWidth;
        gameTop = gameHeight;
        gameBottom = 0;
        gameRight = 0.5f * gameWidth;
        gameLeft = -0.5f * gameWidth;
    }
    void Awake()
    {
        Set();
    }
    void MatchCanvas()
    {
        float scale;
        if (isDriveHeight) scale = WindowHeight / scaleFactor / screenHeight;
        else scale = WindowWidth * screenRatio / scaleFactor / screenHeight;

        canvasScaler.scaleFactor = scale;

        float factor = pixelPerUnit / scale;

        screenRect.anchoredPosition = new Vector2(0, 0.5f * screenHeight) * factor;//
        screenRect.sizeDelta = new Vector2(screenWidth, screenHeight) * factor;

        gameRect.anchoredPosition = new Vector2(0, 0.5f * gameHeight) * factor;
        gameRect.sizeDelta = new Vector2(gameWidth, gameHeight) * factor;
    }
    void Update()
    {
        if (WindowHeight != 0 && WindowWidth != 0 && WindowHeight != prevWindowHeight || WindowWidth != prevWindowWidth)
        {
            Refresh();
            MatchCanvas();

            onScreenResized?.Invoke();

            prevWindowHeight = WindowHeight;
            prevWindowWidth = WindowWidth;
        }
    }
    public void Refresh()
    {
        windowRatio = (float)WindowHeight / WindowWidth;
        
        isDriveHeight = windowRatio < screenRatio;

        pixelPerUnit = isDriveHeight ? (WindowHeight / screenHeight) : (WindowWidth / screenWidth);
    }
    void OnValidate()
    {
        Set();
        Refresh();

        onWindowValidate?.Invoke();
    }
}