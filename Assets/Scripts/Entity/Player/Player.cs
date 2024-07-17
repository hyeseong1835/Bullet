using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class Player : Entity
{
    public class InputData
    {
        public Vector2 moveInput;
        public Vector2 lastMoveInput;

        public Vector2 mouseWorldPos;
        public Vector2 toMouse;
        public Quaternion toMouseRot;
    }

    public static Player instance { get; private set; }

    static GameManager Game => GameManager.instance;
    public override float GetMaxHP() => maxHp;


    [HideInInspector] public Rigidbody2D rigid;
    [HideInInspector] public CircleCollider2D coll;
    [HideInInspector] public Transform weaponHolder;
    Transform look;
    Transform effect;
    ParticleSystem levelUpParticle;


    [Header("Object")]
    public Image weaponUI;

    [Header("Stat")]
    public float maxHp = 10;

    public float damage = 1;
    public float speed = 1;

    public float dash = 2;
    public float dashCoolTime = 0.5f;
    public bool canDash = true;
    [SerializeField] Box moveLock;

    [Header("Data")]
    [SerializeField] List<Effect> effects = new List<Effect>();

    [SerializeField] Weapon weapon;
    [SerializeField] List<Weapon> weaponList = new List<Weapon>();

    public InputData input = new InputData();

    [Header("EXP")]
    public float[] levelUpExp;
    public int level;
    public float exp;

    void Awake()
    {
        instance = this;
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<CircleCollider2D>();
        weaponHolder = transform.Find("WeaponHolder");
        look = transform.Find("Look");
        effect = transform.Find("Effect");
        levelUpParticle = effect.Find("LevelUp").GetComponent<ParticleSystem>();
    }
    void Update()
    {
        if (Game.state == GameState.Play)
        {
            Key();

            if (input.moveInput == Vector2.zero)
            {
                look.rotation = Quaternion.Euler(0, 0, -Mathf.Atan2(input.toMouse.x, input.toMouse.y) * Mathf.Rad2Deg);
            }
            else 
            {
                input.lastMoveInput = input.moveInput;
                
                Move();
            }
            if (canDash && Input.GetMouseButtonDown(1))
            {
                if (input.toMouse.magnitude <= dash) Dash(input.toMouse);
                else Dash(input.toMouse.normalized * dash);
                
            }
            WallCollide();
            UseWeapon();
        }
    }
    void Key()
    {
        input.moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        input.mouseWorldPos = CameraController.instance.cam.ScreenToWorldPoint(Input.mousePosition);
        input.toMouse = (input.mouseWorldPos - (Vector2)transform.position);
        input.toMouseRot = Quaternion.Euler(0, 0, -Mathf.Atan2(input.toMouse.x, input.toMouse.y) * Mathf.Rad2Deg);
    }
    void Move()
    {
        transform.Translate(GameManager.instance.gameSpeed * input.moveInput.normalized * speed * Time.deltaTime);
        look.rotation = Quaternion.Euler(0, 0, -Mathf.Atan2(input.moveInput.x, input.moveInput.y) * Mathf.Rad2Deg);
    }
    void Dash(Vector2 move)
    {
        canDash = false;
        Invoke(nameof(DashCoolDown), dashCoolTime);

        look.rotation = Quaternion.Euler(0, 0, -Mathf.Atan2(input.toMouse.x, input.toMouse.y) * Mathf.Rad2Deg);
        
        foreach (RaycastHit2D hitInfo in Physics2D.CircleCastAll(transform.position, coll.radius, input.toMouse, input.toMouse.magnitude, Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer("Player"))))
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
    void DashCoolDown()
    {
        canDash = true;
    }
    void WallCollide()
    {
        Vector2 contact;
        if (moveLock.IsContactGame(transform.position, out contact)) transform.position = contact;
    }
    void UseWeapon()
    {
        if (Input.GetMouseButton(0))
        {
            if (weapon == null) return;

            weapon.TryUse();
        }
    }
    public void AddExp(float amount)
    {
        exp += amount;
        for (int levelIndex = level + 1; levelIndex < levelUpExp.Length; levelIndex++)
        {
            if (exp < levelUpExp[levelIndex]) break;
            
            LevelUp();
        }
    }
    public void LevelUp()
    {
        level++;
        
        maxHp *= 1.1f;
        hp = maxHp;

        damage *= 1.1f;
        levelUpParticle.Play();//

        Debug.Log($"Level Up! ({level})");
    }
    public void LevelDown()
    {
        level--;

        maxHp /= 1.1f;

        if (hp > maxHp) hp = maxHp;

        damage /= 1.1f;

        Debug.Log($"Level Down! ({level})");
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
