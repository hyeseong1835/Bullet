using System;
using UnityEngine;

public abstract class Enemy : Entity
{
    public override EntityData EntityData => EnemyData;
    public abstract EnemyData EnemyData { get; set; }
    public abstract EnemySpawnData EnemySpawnData { get; set; }
    public abstract Type EnemySpawnDataType { get; }

    protected void OnEnable()
    {
        GameManager.instance.enableEnemyList.Add(this);
    }
    protected void OnDisable()
    {
        GameManager.instance.enableEnemyList.Remove(this);
    }
    protected void Drop()
    {
        float random = UnityEngine.Random.Range(0, EnemyData.ratioMax);
        foreach (DropInfo drop in EnemyData.drops)
        {
            if (random <= drop.ratio)
            {
                if (drop.prefabs != null)
                {
                    foreach (GameObject prefab in drop.prefabs)
                    {
                        GameObject instance = Instantiate(prefab);
                        instance.transform.position = transform.position;
                    }
                }
                if (drop.items != null)
                {
                    foreach (ItemData dropables in drop.items)
                    {
                        Item instance = dropables.Drop();
                        instance.transform.position = transform.position;
                    }
                }
                break;
            }
            random -= drop.ratio;
        }
        PlayerController.instance.AddExp(EnemyData.exp);
    }
    protected override void OnDead()
    {
        Drop();
        PoolHolder.instance.pools.Find((pool) => (pool.prefab.name == gameObject.name)).DeUse(gameObject);
    }
    protected void OnValidate()
    {
        //gameObject.layer = LayerMask.NameToLayer("Enemy");
    }
}