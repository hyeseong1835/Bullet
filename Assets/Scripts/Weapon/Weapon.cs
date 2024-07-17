using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public GameObject UI;
    
    public bool canUse = true;
    public float cooltime;

    protected void OnEnable()
    {
        UI.SetActive(true);
    }
    protected void OnDisable()
    {
        UI?.SetActive(false);
    }

    public virtual bool TryUse()
    {
        if (canUse)
        {
            StartCoroutine(CoolTime(cooltime));
            Use();
            return true;
        }
        return false;
    }
    IEnumerator CoolTime(float time)
    {
        canUse = false;

        yield return new WaitForSeconds(time);

        canUse = true;
    }
    protected abstract void Use();
}
