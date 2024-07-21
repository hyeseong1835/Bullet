using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SkillChargeBar : MonoBehaviour
{
    Slider slider;

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
        slider.value = Player.instance.skillCharge / Player.instance.skillChargeMax;
    }
}
