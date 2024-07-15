using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public abstract class Item : MonoBehaviour
{
    public abstract ItemData ItemData { get; set; }

    void Start()
    {
        if (ItemData.pool.holder == null) ItemData.pool.Init();
    }
    void Update()
    {
        if (GameManager.IsEditor)
        {
            gameObject.layer = LayerMask.NameToLayer("Item");

            Rigidbody2D rigid = GetComponent<Rigidbody2D>();
            rigid.bodyType = RigidbodyType2D.Kinematic;

            CircleCollider2D collider = GetComponent<CircleCollider2D>();
            collider.isTrigger = true;
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        OnPickup();

        ItemData.pool.DeUse(gameObject);
    }
    protected abstract void OnPickup();
}
