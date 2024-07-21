using UnityEditor;
using UnityEngine;

public class Shooter : Weapon
{
    static Player player => Player.instance;

    [SerializeField] BulletData bulletData;

    [SerializeField] Transform look;
    [SerializeField] Transform tip;

    [HideInInspector] public Pool bulletPool;
    public GameObject bulletPrefab;

    [SerializeField] float chargeOnHit;

#if UNITY_EDITOR
    void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnEditorPlayModeStateChanged;
    }
    void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnEditorPlayModeStateChanged;
    }
    void OnEditorPlayModeStateChanged(PlayModeStateChange state)
    {
        switch (state)
        {
            case PlayModeStateChange.ExitingPlayMode:
                bulletPool = null;
                break;
        }
    }
#endif
    void Start()
    {
        
    }
    public override void Use()
    {
        if (bulletPool == null)
        {
            bulletPool = new Pool(
                PoolType.PlayerBullet,
                bulletPrefab,
                0,
                0
            );
        }

        look.transform.rotation = player.input.toMouseRot;

        GameObject obj = bulletPool.Get();
        obj.transform.position = tip.position;
        obj.transform.rotation = player.input.toMouseRot;

        Bullet bullet = obj.GetComponent<Bullet>();
        bullet.data = bulletData;
        bullet.Initialize(
            BulletUpdate,
            BulletDeUse,
            BulletEnter,
            null,
            null
        );    

        bulletPool.Use(bulletPool.objects.Count - 1, obj);
    }
    void BulletUpdate(Bullet bullet)
    {
        bullet.transform.position += bullet.transform.up * bullet.data.speed * GameManager.deltaTime;

        if (bullet.coll.ToBox().IsExitGame(bullet.transform.position))
        {
            bullet.DeUse();
        }
    }
    void BulletDeUse(Bullet bullet)
    {
        bulletPool.DeUse(bullet.gameObject);
    }
    void BulletEnter(Bullet bullet, Collider2D coll)
    {
        Entity entity = coll.GetComponent<Entity>();
        entity.TakeDamage(bullet.data.damage * player.damage);

        bullet.DeUse();
        player.skillCharge += chargeOnHit;
    }
    public override void Skill()
    {

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(tip.position, tip.position + 0.1f * tip.up);
    }
}
