using System;
using UnityEngine;

public abstract class Enemy : Entity
{
    public enum EnemyState
    {
        Disable, Enable
    }
    public EnemyState entityState = EnemyState.Disable;
    
    public override float GetMaxHP() => EnemyData.maxHp;
    public abstract EnemyData EnemyData { get; set; }
    public abstract EnemySpawnData EnemySpawnData { get; set; }
    public abstract Type EnemySpawnDataType { get; }

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
                    foreach (ItemData itemData in drop.items)
                    {
                        Item instance = itemData.Drop(transform.position);
                    }
                }
                break;
            }
            random -= drop.ratio;
        }
        Player.instance.AddExp(EnemyData.exp);
    }
    protected override void OnDead()
    {
        Drop();
        PoolHolder.instance.pools.Find((pool) => (pool.prefab.name == gameObject.name)).DeUse(gameObject);
    }
}