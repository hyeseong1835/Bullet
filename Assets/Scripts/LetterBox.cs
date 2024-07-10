using UnityEngine;

[ExecuteAlways]
public class LetterBox : MonoBehaviour, IOnScreenResizedReceiver
{
    public GameObject top, right, left;

    void OnEnable()
    {
        Window.onScreenResizedRecieverList.Add(this);
    }
    void OnDisable()
    {
        Window.onScreenResizedRecieverList.Remove(this);
    }
    void Refresh()
    {
        if (Window.isDriveHeight)
        {
            top.SetActive(false);

            right.SetActive(true);
            right.transform.localScale = new Vector3(0.5f * (CameraController.instance.viewSize.x - Window.ScreenWidth), CameraController.instance.viewSize.y, 1);
            right.transform.position = new Vector3(0.5f * CameraController.instance.viewSize.x - 0.5f * right.transform.localScale.x, 0.5f * right.transform.localScale.y, 0);

            left.SetActive(true);
            left.transform.localScale = new Vector3(0.5f * (CameraController.instance.viewSize.x - Window.ScreenWidth), CameraController.instance.viewSize.y, 1);
            left.transform.position = new Vector3(-(0.5f * CameraController.instance.viewSize.x - 0.5f * left.transform.localScale.x), 0.5f * left.transform.localScale.y, 0);
        }
        else
        {
            right.SetActive(false);
            left.SetActive(false);

            top.SetActive(true);
            top.transform.localScale = new Vector3(CameraController.instance.viewSize.x, CameraController.instance.viewSize.y - Window.screenUp, 1);
            top.transform.position = new Vector3(0, Window.ScreenHeight + 0.5f * top.transform.localScale.y, 0);
        }
    }
    public void OnScreenResized()
    {
        Refresh();
    }
}
