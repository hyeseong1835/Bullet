using UnityEngine;
using UnityEngine.UI;

public class ExpBar : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] Slider slider;
    void Awake()
    {
        slider = GetComponent<Slider>();
    }
    void Update()
    {
        Refresh();
    }
    void Refresh()
    {
        if (player == null)
        {
            Debug.LogError("Player is null");
        }
        else
        {
            if (player.level >= player.data.levelUpExp.Length)
            {
                slider.value = 1;
                return;
            }
            float under = player.data.levelUpExp[player.level];
            float up = player.data.levelUpExp[player.level + 1];

            slider.value = (player.exp - under) / (up - under);
        }
    }
    void OnValidate()
    {
        slider = GetComponent<Slider>();

        Refresh();
    }
}
