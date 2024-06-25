using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : Entity
{
    public static PlayerController instance;
    static CameraController cam => CameraController.instance;

    [HideInInspector] public Rigidbody2D rigid;
    [HideInInspector] public Collider2D coll;

    [SerializeField] float speed = 1;
    
    [SerializeField] Vector2 moveLockSize;
    [SerializeField] Vector2 moveLockCenter;

    [SerializeField] Pool bulletPool;

    void Awake()
    {
        instance = this;
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();

    }
    void Start()
    {
        bulletPool.Init((obj) => obj.GetComponent<Bullet>().Initialize(enter: BulletEnter));
    }
    void Update()
    {
        if (EditorApplication.isPlaying == false)
        {
            if (instance == null) instance = this;
            return;
        }
        Move();
        WallCollide();

        if (Input.GetMouseButtonDown(0))
        {
            bulletPool.Use().transform.position = transform.position;
        }
        for (int bulletIndex = 0; bulletIndex < bulletPool.count; bulletIndex++)
        {
            GameObject bulletObj = bulletPool.pool[bulletIndex];

            if (bulletObj.activeInHierarchy)
            {
                bulletObj.transform.position += Vector3.up * 1 * Time.deltaTime;
                if (bulletObj.transform.position.y > cam.height + 1)
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
        if (transform.position.x > (offset = 1 - 0.5f * moveLockSize.x - moveLockCenter.x))
        {
            transform.position = new Vector3(offset, transform.position.y, transform.position.z);
        }
        //ÁÂ
        else if(transform.position.x < (offset = -1 + 0.5f * moveLockSize.x - moveLockCenter.x))
        {
            transform.position = new Vector3(offset, transform.position.y, transform.position.z);
        }
        //À§
        if (transform.position.y > (offset = cam.height - 0.5f * moveLockSize.y - moveLockCenter.y))
        {
            transform.position = new Vector3(transform.position.x, offset, transform.position.z);
        }
        //¾Æ·¡
        else if (transform.position.y < (offset = 0.5f * moveLockSize.y - moveLockCenter.y))
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
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)transform.position + moveLockCenter, moveLockSize);
    }
}
