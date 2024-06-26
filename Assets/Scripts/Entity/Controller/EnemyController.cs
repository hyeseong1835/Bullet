using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyController : Entity
{
    public EnemyControllerData data;
    public override EntityData EntityData => data;

    [SerializeField] float move;

    // Start is called before the first frame update
    void Start()
    {
        move = data.speed;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlaying == false)
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        }
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
