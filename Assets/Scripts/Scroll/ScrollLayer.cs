using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class ScrollLayer : MonoBehaviour
{
    ScrollManager scrollManager => ScrollManager.instance;

    public IScroll[] objArray;
    
    public float speed;

    void Awake()
    {
        if (scrollManager == null) ScrollManager.instance = transform.parent.GetComponent<ScrollManager>();

        SetObjArray();
    }
    void Update()
    {
        if (EditorApplication.isPlaying)
        {

        }
        else
        {
            transform.position = Vector3.zero;
            if (scrollManager == null) ScrollManager.instance = transform.parent.GetComponent<ScrollManager>();
            SetObjArray();
            if (speed < 0)
            {
                speed = 0;
                Debug.LogWarning("Speed cannot be less than 0");
            }
        }
    }
    public void Scroll(float scroll)
    {
        foreach (IScroll obj in objArray)
        {
            obj.Scroll(scroll * speed);
        }
    }
    public void OnTransformChildrenChanged()
    {
        SetObjArray();
    }
    void SetObjArray()
    {
        objArray = transform.GetComponentsInChildren<IScroll>();
    }
}
