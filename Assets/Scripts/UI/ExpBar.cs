using UnityEngine;
using UnityEngine.UI;

public class ExpBar : MonoBehaviour
{
    Player player => Player.instance;
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
        if (player != null && player.levelUpExp != null)
        {
            if (player.level + 1 >= player.levelUpExp.Length)
            {
                slider.value = 1;
                return;
            }
            float under = player.levelUpExp[player.level];
            float up = player.levelUpExp[player.level + 1];

            slider.value = (player.exp - under) / (up - under);
        }
    }
}
