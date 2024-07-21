using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollGroupElement : MonoBehaviour
{
    public float height;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(
            transform.position + Vector3.up * 0.5f * height, new Vector3(Window.instance.gameWidth, height, 0)
        );
    }
}
