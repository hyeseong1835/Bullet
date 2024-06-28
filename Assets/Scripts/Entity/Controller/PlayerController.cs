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

    static Window GameCanvas => Window.instance;
    static GameManager Game => GameManager.instance;

    [HideInInspector] public Rigidbody2D rigid;
    [HideInInspector] public Collider2D coll;

    [SerializeField] float speed = 1;

    [SerializeField] Box moveLock;

    [SerializeField] Weapon weapon;
    [SerializeField] List<Weapon> autoWeaponList = new List<Weapon>();

    public float exp;
    public int level;

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
        Vector3 contact;
        if (moveLock.IsContactGame(transform.position, out contact)) transform.position = contact;
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
    public void AddExp(float amount)
    {
        exp += amount;
        for (int levelIndex = level + 1; levelIndex < data.levelUpExp.Length; levelIndex++)
        {
            if (exp >= data.levelUpExp[levelIndex]) LevelUp();
            else break;
        }
    }
    void LevelUp()
    {
        level++;
        Debug.Log("Level Up! " + level);
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
