using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpBar : MonoBehaviour
{
    Player player => Player.instance;
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI text;
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
        if (player.level + 1 < player.levelUpExp.Length)
        {
            text.text = $"{player.level}: {player.exp.ToString("F0")}/{player.levelUpExp[player.level + 1].ToString("F0")}";
        }
        else
        {
            text.text = $"{player.level}: Max";
        }
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
