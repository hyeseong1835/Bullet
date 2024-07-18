using UnityEngine;

[ExecuteInEditMode]
public class SetSortingLayer : MonoBehaviour
{
    Renderer render;
    public string sortingLayer;
    public int sortingOrder;

    void Start()
    {
        SetLayer();
    }


    public void SetLayer()
    {
        if (render == null)
        {
            render = this.GetComponent<Renderer>();
        }

        render.sortingLayerName = sortingLayer;
        render.sortingOrder = sortingOrder;

        Debug.Log($"Set Sorting Layer\nLater: {render.sortingLayerName}, \nOrder: {render.sortingOrder}");
    }
    void OnValidate()
    {
        SetLayer();
    }
}