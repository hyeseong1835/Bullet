using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public abstract class Item : MonoBehaviour
{
    public abstract ItemData ItemData { get; set; }

    public Vector2 velocity = Vector2.zero;

    protected void OnEnable()
    {
        if (new Box(Vector2.one * 1.5f, Vector2.zero).IsContactGame(transform.position, out Vector2 contact))
        {
            transform.position = contact;
        }
        velocity = (Player.instance.transform.position - transform.position).normalized;
    }
    protected void Start()
    {
        if (ItemData.pool.holder == null) ItemData.pool.Init();
    }
    protected void Update()
    {
        transform.Translate(velocity * Time.deltaTime);

        if (new Box(Vector2.one * 0.25f, Vector2.zero).IsContactGame(transform.position, out Vector2 contact, out int horizontal, out int vertical))
        {
            transform.position = contact;

            if (horizontal != 0) velocity.x = -horizontal * Mathf.Abs(velocity.x);
            if (vertical != 0) velocity.y = -vertical * Mathf.Abs(velocity.y);
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        OnPickup();
    }
    
    protected abstract void OnPickup();

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        CustomGizmos.DrawVector(
            transform.position, 
            velocity, 
            45 * Mathf.Deg2Rad, 
            0.25f
        );
    }
}
