using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    PlayerController player => PlayerController.instance;
    [SerializeField] float radius = 1;

    void Update()
    {
        if((transform.position - player.transform.position).magnitude <= radius)
        {
            Get();
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    public abstract void Get();
}
