using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class ScrollObject : MonoBehaviour, IScroll
{
    public float jump;
    public float line;

    public void Scroll(float scroll)
    {
        transform.position -= Vector3.up * scroll;

        if (transform.position.y < line)
        {
            transform.position += Vector3.up * jump;
        }
    }
}
