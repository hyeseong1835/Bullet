using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyController : Enemy
{
    public EnemyControllerData data;
    public override EnemyData EnemyData => data;

    [SerializeField] float move;

    void Start()
    {
        move = data.speed;
    }

    new void Update()
    {
        base.Update();
#if UNITY_EDITOR
        if (EditorApplication.isPlaying == false) return;
#endif
        Move();
    }
    void Move()
    {
        if (transform.position.x < -0.8f) move = data.speed;
        else if (transform.position.x > 0.8f) move = -data.speed;
        
        transform.position += Vector3.right * move * Time.deltaTime;
    }
}
