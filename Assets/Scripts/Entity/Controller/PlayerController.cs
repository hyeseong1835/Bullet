using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : Entity
{
    public static PlayerController instance;

    public PlayerControllerData data;
    public override EntityData EntityData => data;

    static GameCanvas GameCanvas => GameCanvas.instance;
    static GameManager Game => GameManager.instance;

    [HideInInspector] public Rigidbody2D rigid;
    [HideInInspector] public Collider2D coll;

    [SerializeField] float speed = 1;

    [SerializeField] Box moveLock;

    [SerializeField] Weapon weapon;
    [SerializeField] List<Weapon> autoWeaponList = new List<Weapon>();
    void Awake()
    {
        if (GameManager.IsEditor) return;

        instance = this;
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }
    void Update()
    {
        if (GameManager.IsEditor)
        {
            gameObject.layer = LayerMask.NameToLayer("Player");

            return;
        }

        if (Game.state == GameState.Play)
        {
            Move();
            WallCollide();
            UseWeapon();
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
    void UseWeapon()
    {
        if (Input.GetMouseButton(0))
        {
            if (weapon == null) return;

            weapon.TryUse();
        }
        foreach (Weapon autoWeapon in autoWeaponList)
        {
            autoWeapon.TryUse();
        }
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
