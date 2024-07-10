using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public interface IOnWindowValidateReceiver
{
    void OnWindowValidate();
}
public interface IOnScreenResizedReceiver
{
    public void OnScreenResized();
}
[ExecuteAlways]
public class Window : MonoBehaviour
{
    public static Window instance;
    public Canvas windowCanvas;
    public RectTransform screenRect;
    public RectTransform gameRect;
    
    public CanvasScaler canvasScaler;

    public static int WindowHeight => Screen.height;
    public static int WindowWidth => Screen.width;
    /// <summary>
    /// Height / Width
    /// </summary>
    public static float windowRatio { get; private set; }

    public static Vector2Int ScreenSize => new Vector2Int(ScreenWidth, ScreenHeight);
    public static int ScreenHeight => instance.screenHeight;
    [SerializeField] int screenHeight = 5;
    public static int ScreenWidth => instance.screenWidth;
    [SerializeField] int screenWidth = 4;
    public static float screenRatio { get; private set; }
    public static float screenUp { get; private set; }
    public static float screenDown { get; private set; }
    public static float screenRight { get; private set; }
    public static float screenLeft { get; private set; }

    public static Vector2Int GameSize => new Vector2Int(GameWidth, GameHeight);
    public static int GameHeight => instance.gameHeight;
    [SerializeField] int gameHeight = 5;
    public static int GameWidth => instance.gameWidth;
    [SerializeField] int gameWidth = 2;
    public static float gameRatio { get; private set; }
    public static float gameUp { get; private set; }
    public static float gameDown { get; private set; }
    public static float gameRight { get; private set; }
    public static float gameLeft { get; private set; }


    public float scaleFactor = 1024;

    public static float pixelPerUnit { get; private set; }

    public static bool isDriveHeight { get; private set; }

    public static List<IOnScreenResizedReceiver> onScreenResizedRecieverList = new List<IOnScreenResizedReceiver>();
    float prevWindowHeight;
    float prevWindowWidth;

    void Set()
    {
        instance = this;

        screenRatio = screenHeight / screenWidth;
        screenUp = screenHeight;
        screenDown = 0;
        screenRight = 0.5f * screenWidth;
        screenLeft = -0.5f * screenWidth;

        gameRatio = gameHeight / gameWidth;
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
            Set();
            Refresh();

            if (onScreenResizedRecieverList != null)
            {
                foreach (IOnScreenResizedReceiver receiver in onScreenResizedRecieverList)
                {
                    receiver.OnScreenResized();
                }
            }

            prevWindowHeight = WindowHeight;
            prevWindowWidth = WindowWidth;
        }
    }
    public void Refresh()
    {
        windowRatio = (float)WindowHeight / WindowWidth;
        
        isDriveHeight = windowRatio < screenRatio;

        pixelPerUnit = isDriveHeight ? (WindowHeight / screenHeight) : (WindowWidth / screenWidth);
        
        float scale;
        if (isDriveHeight) scale = WindowHeight / scaleFactor;
        else scale = WindowWidth * screenRatio / scaleFactor;
        
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

        var onWindowValidaterecieverIt = FindObjectsOfType<MonoBehaviour>()
                                                   .OfType<IOnWindowValidateReceiver>();

        foreach (IOnWindowValidateReceiver receiver in onWindowValidaterecieverIt)
        {
            receiver.OnWindowValidate();
        }
    }
}