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
