using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanvas : MonoBehaviour
{
    static GameCanvas instance;
    static CameraController cam => CameraController.instance;
    public RectTransform rect;

    public float screenRatio => cam.screenRatio;

    public float height;
    public float width;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    void Start()
    {
        cam.onScreenRatioChangeEvent.AddListener(OnChangeScreenRatio);
    }
    public void OnChangeScreenRatio()
    {
        rect.anchoredPosition = new Vector2(0, 0.5f * height * cam.pixelPerUnit);
        rect.sizeDelta = new Vector2(width * cam.pixelPerUnit, height * cam.pixelPerUnit);
    }
}
