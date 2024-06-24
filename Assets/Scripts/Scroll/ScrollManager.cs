using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public interface IScroll
{
    public void Scroll(float scroll);
}
[ExecuteAlways]
public class ScrollManager : MonoBehaviour
{
    public static ScrollManager instance;
    public ScrollLayer[] scrollLayerArray;

    public float speed = 1;

    void Awake()
    {

    }
    void Update()
    {
        if (EditorApplication.isPlaying)
        {
            Scroll(speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.zero;

            SetScrollLayerArray();
        }

    }
    public void Scroll(float scroll)
    {
        foreach (ScrollLayer scrollLayer in scrollLayerArray) 
        {
            scrollLayer.Scroll(scroll);
        }
    }
    void OnTransformChildrenChanged()
    {
        SetScrollLayerArray();
    }
    void SetScrollLayerArray()
    {
        scrollLayerArray = GetComponentsInChildren<ScrollLayer>();
    }
}
