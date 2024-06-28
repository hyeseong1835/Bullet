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
            Debug.LogWarning("�θ� �� WaveTrigger�� Ʈ�������� �ʽ��ϴ�.");
        }

        if (wave != null && wave.transform.IsChildOf(transform) == false)
        {
            wave = null;
            Debug.LogWarning("Wave�� �� ������Ʈ�� �ڽ��̿��� �մϴ�");
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
