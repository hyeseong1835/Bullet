using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public abstract class Enemy : Entity
{
    public override EntityData EntityData => EnemyData;
    public abstract EnemyData EnemyData { get; }

    protected void Update()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlaying == false)
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        }
#endif
    }
    protected void Drop()
    {
        float random = Random.Range(0, EnemyData.ratioMax);
        foreach (DropInfo drop in EnemyData.drops)
        {
            if (random <= drop.ratio)
            {
                if (drop.prefab == null) break;

                GameObject itemInstance = Instantiate(drop.prefab);
                itemInstance.transform.position = transform.position;
                break;
            }
            random -= drop.ratio;
        }
    }
    protected override void OnDead()
    {
        Drop();
        Destroy(gameObject);
    }
}
