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

        transform.position += transform.up * data.speed * Time.deltaTime;

        if (coll.ToBox().IsExitGame(transform.position))
        {
            Destroy();
        }
    }
}
