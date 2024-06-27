using UnityEngine;
using System;
using System.Linq;
public class LetterBox : MonoBehaviour
{
    public Transform top, bottom, right, left;
    [SerializeField] float size;

    void Refresh()
    {
        top.position = new Vector3(0, Window.ScreenHeight + 0.5f * size, 0);
        top.localScale = new Vector3(Window.ScreenWidth, size, 1);

        bottom.position = new Vector3(0, -0.5f * size, 0);
        bottom.localScale = new Vector3(Window.ScreenWidth, size, 1);

        right.position = new Vector3(0.5f * Window.ScreenWidth + 0.5f * size, 0.5f * Window.ScreenHeight, 0);
        right.localScale = new Vector3(size, Window.ScreenHeight + 2 * size, 1);

        left.position = new Vector3(-0.5f * Window.ScreenWidth - 0.5f * size, 0.5f * Window.ScreenHeight, 0);
        left.localScale = new Vector3(size, Window.ScreenHeight + 2 * size, 1);
    }
    void OnValidate()
    {
        SubScribe();
    }
    void SubScribe()
    {
        if (Window.instance.onScreenResized == null)
        {
            Window.instance.onScreenResized = Refresh;
            return;
        }
        Delegate[] delegateArray = Window.instance.onScreenResized.GetInvocationList();

        if (delegateArray.Contains((Action)Refresh) == false)
        {
            Window.instance.onScreenResized += Refresh;
        }
    }
}
