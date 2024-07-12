using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class PlayerController : Entity
{
    public static PlayerController instance;

    public PlayerControllerData data;
    public override EntityData EntityData => data;

    [SerializeField] Transform grafic;

    static Window GameCanvas => Window.instance;
    static GameManager Game => GameManager.instance;

    [HideInInspector] public Rigidbody2D rigid;
    [HideInInspector] public CircleCollider2D coll;
    [SerializeField] Transform weaponHolder;
    [SerializeField] float speed = 1;
    [SerializeField] float dash = 2;

    [SerializeField] Box moveLock;

    [SerializeField] Weapon weapon;
    [SerializeField] List<Weapon> autoWeaponList = new List<Weapon>();

    public float exp;
    public int level;

    Vector2 moveInput;
    Vector2 lastMoveInput;
    Vector2 mouseWorldPos;
    Vector2 toMouse;
    void Awake()
    {
        if (GameManager.IsEditor) return;

        instance = this;
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<CircleCollider2D>();
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
            Key();

            if (moveInput == Vector2.zero)
            {
                grafic.rotation = Quaternion.Euler(0, 0, -Mathf.Atan2(toMouse.x, toMouse.y) * Mathf.Rad2Deg);
            }
            else 
            {
                lastMoveInput = moveInput;
                
                Move();
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (toMouse.magnitude <= dash) Dash(toMouse);
                else Dash(toMouse.normalized * dash);
                
            }
            WallCollide();
            UseWeapon();
        }
    }
    void Key()
    {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        mouseWorldPos = CameraController.instance.cam.ScreenToWorldPoint(Input.mousePosition);
        toMouse = (mouseWorldPos - (Vector2)transform.position);
        weaponHolder.rotation = Quaternion.Euler(0, 0, -Mathf.Atan2(toMouse.x, toMouse.y) * Mathf.Rad2Deg);
    }
    void Move()
    {
        transform.Translate(moveInput.normalized * speed * Time.deltaTime);
        grafic.rotation = Quaternion.Euler(0, 0, -Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg);
    }
    void Dash(Vector2 move)
    {
        grafic.rotation = Quaternion.Euler(0, 0, -Mathf.Atan2(toMouse.x, toMouse.y) * Mathf.Rad2Deg);
        
        foreach (RaycastHit2D hitInfo in Physics2D.CircleCastAll(transform.position, coll.radius, toMouse, toMouse.magnitude, Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("Player"))))
        {
            switch (hitInfo.collider.gameObject.layer)
            {
                case 20:
                    hitInfo.collider.GetComponent<Item>().OnTriggerEnter2D(coll);
                    break;
            }    
        }
        
        transform.Translate(move);
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
            if (exp < data.levelUpExp[levelIndex]) break;
            
            LevelUp();
        }
    }
    void LevelUp()
    {
        level++;
        Debug.Log($"Level Up! ({level})");
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
