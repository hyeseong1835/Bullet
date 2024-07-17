using UnityEngine;

public class Shooter : Weapon
{
    static Player player => Player.instance;

    [SerializeField] Transform look;
    [SerializeField] Transform tip;
    
    Bullet bullet;
    public GameObject bulletPrefab;

    public float damage;
    public float speed;

    void Awake()
    {
        bullet = bulletPrefab.GetComponent<Bullet>();
        
        if (bullet.data.pool.holder == null) bullet.data.pool.Init();
    }
    protected override void Use()
    {
        look.transform.rotation = player.input.toMouseRot;

        GameObject obj = bullet.data.pool.Use();
        obj.transform.position = tip.position;
        obj.transform.rotation = tip.rotation;

        obj.GetComponent<Bullet>().Initialize(
            (bullet) => {
                transform.position += GameManager.instance.gameSpeed * transform.up * speed * Time.deltaTime;

                if (bullet.coll.ToBox().IsExitGame(transform.position))
                {
                    bullet.DeUse();
                }
            },
            (bullet) => {
                bullet.data.pool.DeUse(gameObject);
            },
            (bullet, coll) =>
            {
                Entity entity = coll.GetComponent<Entity>();
                entity.TakeDamage(damage * player.damage);

                bullet.DeUse();
            },
            null,
            null
        );            
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(tip.position, tip.position + 0.1f * tip.up);
    }
}
