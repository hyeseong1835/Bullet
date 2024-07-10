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
    public float windowRatio;

    public Vector2 ScreenSize => new Vector2(screenWidth, screenHeight);
    public float screenHeight = 5;
    public float screenWidth = 4;
    public float screenRatio;
    public float screenUp;
    public float screenDown;
    public float screenRight;
    public float screenLeft;

    public Vector2Int GameSize => new Vector2Int(gameWidth, gameHeight);
    public int gameHeight = 5;
    public int gameWidth = 2;
    public float gameRatio; 
    public float gameUp;
    public float gameDown;
    public float gameRight;
    public float gameLeft;


    public float scaleFactor = 1024;

    public float pixelPerUnit;

    public bool isDriveHeight;

    public List<IOnScreenResizedReceiver> onScreenResizedRecieverList = new List<IOnScreenResizedReceiver>();
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

        gameRatio = (float)gameHeight / gameWidth;
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
        if (WindowHeight != 0 && WindowWidth != 0 && WindowHeight != prevWindowHeight || WindowWidth != prevWindowWidth)
        {
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
        Refresh();

        var onWindowValidaterecieverIt = FindObjectsOfType<MonoBehaviour>()
                                                   .OfType<IOnWindowValidateReceiver>();

        foreach (IOnWindowValidateReceiver receiver in onWindowValidaterecieverIt)
        {
            receiver.OnWindowValidate();
        }
    }
}