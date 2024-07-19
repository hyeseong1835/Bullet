using UnityEngine;

public interface IScroll
{
    public void Scroll(float scroll);
}
public class ScrollManager : MonoBehaviour
{
    public static ScrollManager instance;

    public ScrollLayer[] scrollLayerArray;

    public float speed = 1;

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
