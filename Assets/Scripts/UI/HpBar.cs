using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HpBar : MonoBehaviour
{
    public Entity target;
    [SerializeField] GameObject grafic;
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
        if (target == null)
        {
            grafic.SetActive(false);
        }
        else
        {
            grafic.SetActive(true);
            slider.value = target.hp / target.GetMaxHP();
        }
    }
}
