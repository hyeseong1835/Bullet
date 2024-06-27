using UnityEngine;

[ExecuteAlways]
public class LetterBox : MonoBehaviour
{
    static Window GameCanvas => Window.instance;

    public Transform top, bottom, right, left;
    [SerializeField] float size;

    void Update()
    {
        Window.instance.onScreenResized += Refresh;
    }
    public void Refresh()
    {
        if (GameCanvas == null) return;

        top.position = new Vector3(0, GameCanvas.screenHeight + 0.5f * size, 0);
        top.localScale = new Vector3(GameCanvas.screenWidth, size, 1);

        bottom.position = new Vector3(0, -0.5f * size, 0);
        bottom.localScale = new Vector3(GameCanvas.screenWidth, size, 1);

        right.position = new Vector3(0.5f * GameCanvas.screenWidth + 0.5f * size, 0.5f * GameCanvas.screenHeight, 0);
        right.localScale = new Vector3(size, GameCanvas.screenHeight + 2 * size, 1);

        left.position = new Vector3(-0.5f * GameCanvas.screenWidth - 0.5f * size, 0.5f * GameCanvas.screenHeight, 0);
        left.localScale = new Vector3(size, GameCanvas.screenHeight + 2 * size, 1);
    }
}
