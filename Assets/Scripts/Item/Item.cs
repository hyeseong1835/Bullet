using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public abstract class Item : MonoBehaviour
{
    protected PlayerController player => PlayerController.instance;
    protected Inventory inventory => player.inventory;

    void Update()
    {
#if UNITY_EDITOR
        if (Application.isPlaying == false)
        {
            gameObject.layer = LayerMask.NameToLayer("Item");

            Rigidbody2D rigid = GetComponent<Rigidbody2D>();
            rigid.bodyType = RigidbodyType2D.Kinematic;

            CircleCollider2D collider = GetComponent<CircleCollider2D>();
            collider.isTrigger = true;
        }
#endif
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        OnPickup();

        Destroy(gameObject);
    }
    protected abstract void OnPickup();
}
