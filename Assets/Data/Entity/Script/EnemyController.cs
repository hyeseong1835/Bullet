using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : Entity
{
    [SerializeField] float speed;
    [SerializeField] float move;

    // Start is called before the first frame update
    void Start()
    {
        move = speed;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    void Move()
    {
        if (transform.position.x < -0.8f) move = speed;
        else if (transform.position.x > 0.8f) move = -speed;
        
        transform.position += Vector3.right * move * Time.deltaTime;
    }
}
