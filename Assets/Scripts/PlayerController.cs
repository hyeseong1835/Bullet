using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    static CameraController cam => CameraController.instance;

    public Rigidbody2D rigid;
    public Collider2D coll;
    public float speed = 1;

    public int hp = 3;

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
        Collide();
    }
    void Move()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        rigid.AddForce(input.normalized * speed * Time.deltaTime, ForceMode2D.Force);
    }
    public void Hit(int damage)
    {
        hp -= damage;
    }
    void Collide()
    {
        /*
        Vector2 leftBottom = cam.leftBottom + (Vector2)coll.bounds.size * 0.5f;
        Vector2 rightTop = cam.rightTop - (Vector2)coll.bounds.size * 0.5f;
        //¿ì
        if (transform.position.x > rightTop.x)
        {
            transform.position = new Vector3(rightTop.x, transform.position.y, transform.position.z);
            if (rigid.velocity.x > 0) 
            {
                rigid.velocity = new Vector3(0, rigid.velocity.y, 0);
            }
        }
        //ÁÂ
        else if(transform.position.x < leftBottom.x)
        {
            transform.position = new Vector3(leftBottom.x, transform.position.y, transform.position.z);
            if (rigid.velocity.x < 0)
            {
                rigid.velocity = new Vector3(0, rigid.velocity.y, 0);
            }
        }
        //À§
        if (transform.position.y > rightTop.y)
        {
            transform.position = new Vector3(transform.position.x, rightTop.y, transform.position.z);
            if (rigid.velocity.y > 0)
            {
                rigid.velocity = new Vector3(rigid.velocity.x, 0, 0);
            }
        }
        //¾Æ·¡
        else if (transform.position.y < leftBottom.y)
        {
            transform.position = new Vector3(transform.position.x, leftBottom.y, transform.position.z);
            if (rigid.velocity.y < 0)
            {
                rigid.velocity = new Vector3(rigid.velocity.x, 0, 0);
            }
        }
        */
    }
}
