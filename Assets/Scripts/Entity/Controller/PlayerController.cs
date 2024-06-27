using UnityEditor;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : Entity
{
    public PlayerControllerData data;
    public override EntityData EntityData => data;

    public static PlayerController instance;
    static CameraController cam => CameraController.instance;
    static GameCanvas GameCanvas => GameCanvas.instance;
    static GameManager Game => GameManager.instance;

    [HideInInspector] public Rigidbody2D rigid;
    [HideInInspector] public Collider2D coll;

    public Inventory inventory;

    [SerializeField] float speed = 1;

    [SerializeField] Box moveLock;

    [SerializeField] Pool bulletPool;
    [SerializeField] float bulletSpeed = 5;

    void Awake()
    {
        if (GameManager.isEditor) return;

        instance = this;
        inventory = GetComponent<Inventory>();
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }
    void Start()
    {
        if (GameManager.isEditor) return;

        bulletPool.Init((obj) => obj.GetComponent<Bullet>().Initialize(enter: BulletEnter));
    }
    void Update()
    {
        if (GameManager.isEditor)
        {
            gameObject.layer = LayerMask.NameToLayer("Player");

            return;
        }

        if (Game.state == GameState.Play)
        {
            Move();
            WallCollide();

            if (Input.GetMouseButtonDown(0))
            {
                bulletPool.Use().transform.position = transform.position;
            }
            BulletMove();
        }
    }
    void BulletMove()
    {
        for (int bulletIndex = 0; bulletIndex < bulletPool.count; bulletIndex++)
        {
            GameObject bulletObj = bulletPool.pool[bulletIndex];

            if (bulletObj.activeInHierarchy)
            {
                bulletObj.transform.position += Vector3.up * bulletSpeed * Time.deltaTime;
                if (bulletObj.transform.position.y > GameCanvas.height + 1)
                {
                    bulletPool.DeUse(bulletObj);
                }
            }
        }
    }
    void Move()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        transform.Translate(input.normalized * speed * Time.deltaTime);
    }
    void WallCollide()
    {
        float offset;

        //¿ì
        if (transform.position.x > (offset = 1 - 0.5f * moveLock.size.x - moveLock.center.x))
        {
            transform.position = new Vector3(offset, transform.position.y, transform.position.z);
        }
        //ÁÂ
        else if(transform.position.x < (offset = -1 + 0.5f * moveLock.size.x - moveLock.center.x))
        {
            transform.position = new Vector3(offset, transform.position.y, transform.position.z);
        }
        //À§
        if (transform.position.y > (offset = GameCanvas.height - 0.5f * moveLock.size.y - moveLock.center.y))
        {
            transform.position = new Vector3(transform.position.x, offset, transform.position.z);
        }
        //¾Æ·¡
        else if (transform.position.y < (offset = 0.5f * moveLock.size.y - moveLock.center.y))
        {
            transform.position = new Vector3(transform.position.x, offset, transform.position.z);
        }
    }
    void BulletEnter(Bullet bullet, Collider2D coll)
    {
        Entity entity = coll.GetComponent<Entity>();
        entity.TakeDamage(1);

        bulletPool.DeUse(bullet.gameObject);
    }
    void OnValidate()
    {
        instance = this;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)transform.position + moveLock.center, moveLock.size);
    }
}
