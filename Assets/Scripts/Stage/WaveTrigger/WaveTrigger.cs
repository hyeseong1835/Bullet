using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class WaveTrigger : MonoBehaviour
{
    public GameObject wave;

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
