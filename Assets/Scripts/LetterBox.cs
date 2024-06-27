using UnityEngine;

public class LetterBox : MonoBehaviour, IOnWindowValidateReceiver
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
    public void OnWindowValidate()
    {
        Refresh();
    }
}
