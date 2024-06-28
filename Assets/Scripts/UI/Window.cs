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

    public static float pixelPerUnit { get; private set; }

    public static bool isDriveHeight { get; private set; }

    IOnScreenResizedReceiver[] onScreenResizedReceivers;

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

        var onWindowValidaterecieverIt = FindObjectsOfType<MonoBehaviour>()
                                                   .OfType<IOnWindowValidateReceiver>();

        foreach (IOnWindowValidateReceiver receiver in onWindowValidaterecieverIt)
        {
            receiver.OnWindowValidate();
        }

        var onScreenResizedRecieverIt = FindObjectsOfType<MonoBehaviour>()
                                                  .OfType<IOnScreenResizedReceiver>();

        onScreenResizedReceivers = onScreenResizedRecieverIt.ToArray();
    }
    void Update()
    {
        if (WindowHeight != prevWindowHeight || WindowWidth != prevWindowWidth)
        {
            windowRatio = (float)WindowHeight / WindowWidth;
            
            Refresh();

            foreach (IOnScreenResizedReceiver receiver in onScreenResizedReceivers)
            {
                receiver.OnScreenResized();
            }

            prevWindowHeight = WindowHeight;
            prevWindowWidth = WindowWidth;
        }
    }
    public void Refresh()
    {
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

        var onScreenResizedRecieverIt = FindObjectsOfType<MonoBehaviour>()
                                                  .OfType<IOnScreenResizedReceiver>();

        onScreenResizedReceivers = onScreenResizedRecieverIt.ToArray();
    }
}