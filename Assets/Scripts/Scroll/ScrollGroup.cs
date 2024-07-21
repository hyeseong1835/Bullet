using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[ExecuteAlways]
public class ScrollGroup : MonoBehaviour, IScroll
{
    public ScrollLayer layer;
    [ReadOnly(true)] public ScrollGroupElement[] elementArray;
    public ScrollGroupElement bottomElement;
    public int bottomElementIndex;

    public float height;

    public void Scroll(float scroll)
    {
        bottomElement.transform.position -= Vector3.up * scroll;
        height = bottomElement.height;
        
        for (int i = 1; i < elementArray.Length; i++)
        {
            int index;
            if (bottomElementIndex + i < elementArray.Length)
            {
                index = bottomElementIndex + i;
            }
            else index = i + bottomElementIndex - elementArray.Length;

            ScrollGroupElement element = elementArray[index];
            element.transform.position = new Vector3(
                    element.transform.position.x, 
                    bottomElement.transform.position.y + height, 
                    element.transform.position.z
                );
            height += element.height;
        }
        NextBottomElement();
    }
    void NextBottomElement()
    {
        if (bottomElement.transform.position.y + bottomElement.height >= 0) return;

        bottomElement.transform.position += Vector3.up * height;
        
        if (bottomElementIndex >= elementArray.Length - 1) bottomElementIndex = 0;
        else bottomElementIndex++;
        
        bottomElement = elementArray[bottomElementIndex];

        NextBottomElement();
    }

    void SetElementArray()
    {
        bottomElementIndex = 0;

        elementArray = new ScrollGroupElement[transform.childCount];

        if (transform.childCount >= 1)
        {
            bottomElement = transform.GetChild(0).GetComponent<ScrollGroupElement>();
            elementArray[0] = bottomElement;

            height = 0;

            for (int i = 0; i < transform.childCount; i++)
            {
                ScrollGroupElement element = transform.GetChild(i).GetComponent<ScrollGroupElement>();
                elementArray[i] = element;
                element.transform.position = new Vector3(element.transform.position.x, height + bottomElement.transform.position.y, 0);
                height += element.height;
            }
        }
        else
        {
            bottomElement = null;
            height = 0;
        }

        if(bottomElement.transform.position.y + bottomElement.height < 0)
        {
            bottomElement.transform.position = new Vector3(
                    bottomElement.transform.position.x,
                    bottomElement.transform.position.y + bottomElement.height,
                    bottomElement.transform.position.z
                );
        }

    }
    void OnValidate()
    {
        transform.position = Vector3.zero;
        if (layer == null) layer = transform.parent.GetComponent<ScrollLayer>();

        SetElementArray();
    }
    void OnDrawGizmosSelected()
    {
        if (bottomElement != null)
        {
            Gizmos.DrawWireCube(
                bottomElement.transform.position + Vector3.up * 0.5f * bottomElement.height, 
                new Vector3(Window.instance.gameWidth, bottomElement.height, 0)
            );
        }
    }
}
