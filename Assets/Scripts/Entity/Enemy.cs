using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public abstract class Enemy : Entity
{
    public override EntityData EntityData => EnemyData;
    public abstract EnemyData EnemyData { get; }

    protected void Update()
    {
        if (GameManager.IsEditor)
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        }
    }
    private void OnEnable()
    {
        GameManager.instance.enableEnemyList.Add(this);
    }
    private void OnDisable()
    {
        GameManager.instance.enableEnemyList.Remove(this);
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
        PlayerController.instance.AddExp(EnemyData.exp);
    }
    protected override void OnDead()
    {
        Drop();
        Destroy(gameObject);
    }

    public abstract void DrawStageEditorGUI(Rect rect);
    public abstract float GetStageEditorGUIHeight(Rect rect);
}