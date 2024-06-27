using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightBullet : Bullet
{
    public StraightBulletData data;
    public override BulletData BulletData => data;

    new void Update()
    {
        base.Update();

        Vector3 dir = Vector3.zero;
        dir.x = Mathf.Cos(Mathf.Deg2Rad * transform.rotation.z);
        dir.y = Mathf.Sin(Mathf.Deg2Rad * transform.rotation.z);

        transform.position += dir * speed * Time.deltaTime;

        if (coll.ToBox().IsExitGame(transform.position))
        {
            destroyEvent.Invoke(this);
        }
    }
}
