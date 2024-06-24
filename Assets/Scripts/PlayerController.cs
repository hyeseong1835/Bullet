using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    static CameraController cam => CameraController.instance;

    public Rigidbody2D rigid;
    public Collider2D coll;
    public float speed = 1;

    public int hp = 3;
    public Vector2 moveLockSize;
    public Vector2 moveLockCenter;

    void Awake()
    {
        instance = this;
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
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
    }
    void Move()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        transform.Translate(input.normalized * speed * Time.deltaTime);
    }
    public void Hit(int damage)
    {
        hp -= damage;
    }
    void WallCollide()
    {
        Vector2 offset = new Vector2(-0.5f * moveLockSize.x - moveLockCenter.x, -0.5f * moveLockSize.y - moveLockCenter.y);
       
        //¿ì
        if (transform.position.x > 1 + offset.x)
        {
            transform.position = new Vector3(1 + offset.x, transform.position.y, transform.position.z);
        }
        //ÁÂ
        else if(transform.position.x < -1 - offset.x)
        {
            transform.position = new Vector3(-1 - offset.x, transform.position.y, transform.position.z);
        }
        //À§
        if (transform.position.y > cam.height + offset.y)
        {
            transform.position = new Vector3(transform.position.x, cam.height + offset.y, transform.position.z);
        }
        //¾Æ·¡
        else if (transform.position.y < -offset.y)
        {
            transform.position = new Vector3(transform.position.x, -offset.y, transform.position.z);
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)transform.position + moveLockCenter, moveLockSize);
    }
}
