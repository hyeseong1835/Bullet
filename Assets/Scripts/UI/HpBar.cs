using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HpBar : MonoBehaviour
{
    public Entity target;
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
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
            slider.value = target.hp / target.EntityData.maxHp;
        }
    }
    void OnValidate()
    {
        slider = GetComponent<Slider>();

        Refresh();
    }
}
