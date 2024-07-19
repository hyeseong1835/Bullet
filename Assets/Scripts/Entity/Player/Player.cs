using System.Collections;
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
    
    public Transform weaponHolder { get; private set; }
    public RectTransform effectHolder;
    public Effect[] effects { get; private set; }
    Transform look;
    Transform vfx;
    ParticleSystem levelUpParticle;

    [Header("Object")]
    public Image weaponUI;
    public Image weaponFrame;
    [SerializeField] protected ParticleSystem weaponUIBreakParticle;

    [SerializeField] GameObject level1Grafic;
    [SerializeField] GameObject level2Grafic;
    [SerializeField] GameObject level3Grafic;

    [Header("Stat")]
    public float maxHp = 10;

    public float damage = 1;
    public float speed = 1;

    bool canUse = true;
    
    [SerializeField] Box moveLock;

    [Header("Data")]
    public Weapon defaultWeapon;
    public Weapon weapon;

    public InputData input = new InputData();

    [Header("EXP")]
    public float[] levelUpExp;
    public int level;
    public float exp;

    float weaponBreakTime = 1;
    bool canWeaponBreak = true;

    void Awake()
    {
        instance = this;
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<CircleCollider2D>();
        weaponHolder = transform.Find("WeaponHolder");
        look = transform.Find("Look");
        vfx = transform.Find("VFX");
        {
            levelUpParticle = vfx.Find("LevelUp").GetComponent<ParticleSystem>();
        }
        effects = new Effect[effectHolder.childCount];
        for (int i = 0; i < effectHolder.childCount; i++)
        {
            effects[i] = effectHolder.GetChild(i).GetComponent<Effect>();
        }
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
                WallCollide();
            }

            DestroyWeaponKey();
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
        transform.Translate(input.moveInput.normalized * speed * Time.deltaTime);
        look.rotation = Quaternion.Euler(0, 0, -Mathf.Atan2(input.moveInput.x, input.moveInput.y) * Mathf.Rad2Deg);
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
            
            if (canUse)
            { 
                weapon.Use();
                if (weapon.cooltime != -1) StartCoroutine(WeaponUseCoolTime(weapon.cooltime));
            }
        }
    }
    void DestroyWeaponKey()
    {
        if (weapon == null || weapon == defaultWeapon)
        {
            weaponFrame.fillAmount = 0;
            return;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            if (weaponFrame.fillAmount != 0)
            {
                weaponFrame.fillAmount -= Time.deltaTime / weaponBreakTime;
                if (weaponFrame.fillAmount < 0) weaponFrame.fillAmount = 0;
            }

            if (canWeaponBreak)
            {
                if (weaponFrame.fillAmount == 0)
                {
                    WeaponBreak();
                    canWeaponBreak = false;
                }
            }
        }
        else
        {
            if (weaponFrame.fillAmount != 1)
            {
                weaponFrame.fillAmount += Time.deltaTime / weaponBreakTime * 3;
                if (weaponFrame.fillAmount > 1) weaponFrame.fillAmount = 1;
            }

            if (weaponFrame.fillAmount == 1)
            {
                canWeaponBreak = true;
            }
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
        switch (level)
        {
            case 2:
                level1Grafic.SetActive(false);
                level2Grafic.SetActive(true);
                break;
            case 4:
                level2Grafic.SetActive(false);
                level3Grafic.SetActive(true);
                break;
        }
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
    
    public void GetWeapon(Weapon weapon)
    {
        Debug.Log($"Get Weapon! {weapon.data.prefab.name}[{weapon.data.level}]");
        defaultWeapon.gameObject.SetActive(false);

        this.weapon = weapon;
        weaponUI.sprite = weapon.data.UI;
    }
    public void UpgradeWeapon()
    {
        Debug.Log($"Upgrade Weapon! {weapon.data.upgrade.data[weapon.data.level + 1].prefab.name}[{weapon.data.level + 1}]");
        GameObject weaponObj = Instantiate(
            weapon.data.upgrade.data[weapon.data.level + 1].prefab, 
            weaponHolder
        );
        Weapon weaponInstance = (Weapon)weaponObj.GetComponent(weapon.GetType());

        Destroy(weapon.gameObject);
        weapon = weaponInstance;
        weaponUI.sprite = weapon.data.UI;
    }
    void WeaponBreak()
    {
        ParticleSystem.ShapeModule shape = weaponUIBreakParticle.shape;
        shape.texture = weaponUI.sprite.texture;

        weaponUIBreakParticle.Play();
        Destroy(weapon.gameObject);

        weapon = defaultWeapon;
        weaponUI.sprite = defaultWeapon.data.UI;

        defaultWeapon.gameObject.SetActive(true);
    }

    IEnumerator WeaponUseCoolTime(float time)
    {
        canUse = false;

        yield return new WaitForSeconds(time);

        canUse = true;
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
