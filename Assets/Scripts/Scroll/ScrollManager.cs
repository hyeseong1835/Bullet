using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IScroll
{
    public void Scroll(float scroll);
}
public class ScrollManager : MonoBehaviour
{
    public static ScrollManager instance;

    public List<ScrollLayer> scrollLayerList;

    public float speed = 1;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        SetScrollLayerArray();
    }
    void Update()
    {
        if (GameManager.IsEditor)
        {
            transform.position = Vector3.zero;

            SetScrollLayerArray();
            return;
        }
     
        Scroll(speed * GameManager.deltaTime);
    }
    public void Scroll(float scroll)
    {
        foreach (ScrollLayer scrollLayer in scrollLayerList) 
        {
            scrollLayer.Scroll(scroll);
        }
    }
    void SetScrollLayerArray()
    {
        scrollLayerList = GetComponentsInChildren<ScrollLayer>().ToList();
    }
    private void OnTransformChildrenChanged()
    {
        SetScrollLayerArray();
    }
    void OnValidate()
    {
        SetScrollLayerArray();
    }
}
