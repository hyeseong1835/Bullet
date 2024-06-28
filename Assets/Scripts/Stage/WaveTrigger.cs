using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveTrigger : MonoBehaviour
{
    public GameObject wave;

    [SerializeField] int remainEnemyCount;
    [SerializeField] float time;

    void Update()
    {
        if (GameManager.instance.enableEnemyList.Count <= remainEnemyCount)
        {
            wave.SetActive(true);
            Invoke("Timer", time);
        }
    }
    void Timer()
    {
        wave.SetActive(true);
        Destroy(this);
    }

    void OnValidate()
    {
        if(transform.parent != null && IsParentTriggerThis() == false)
        {
            Debug.LogWarning("부모가 이 WaveTrigger를 트리거하지 않습니다.");
        }

        if (wave != null && wave.transform.IsChildOf(transform) == false)
        {
            wave = null;
            Debug.LogWarning("Wave는 이 오브젝트의 자식이여야 합니다");
        }
        bool IsParentTriggerThis()
        {
            WaveTrigger[] parentTriggerArray = transform.parent.GetComponents<WaveTrigger>();
            for (int i = 0; i < parentTriggerArray.Length; i++)
            {
                if (parentTriggerArray[i].wave == gameObject) return true;
            }
            return false;
        }
    }
    
}
