using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Weapon
{
    static Player player => Player.instance;
    public LaserData data;
    public override WeaponData WeaponData => data;

    public Transform look;
    public GameObject grafic;
    public Transform tip;

    protected override void Use()
    {
        look.transform.rotation = player.input.toMouseRot;


        RaycastHit2D[] hit = Physics2D.BoxCastAll(
            tip.position + 5 * look.eulerAngles,
            new Vector2(data.width, 10),
            look.eulerAngles.z,
            look.eulerAngles,
            0,
            LayerMask.GetMask("Enemy")
        );

        foreach(RaycastHit2D info in hit)
        {
            Enemy enemy = info.collider.GetComponent<Enemy>();
            enemy.TakeDamage(data.damage * Player.instance.damage);
        }
        

        StartCoroutine(Grafic());
    }
    IEnumerator Grafic()
    {
        grafic.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        grafic.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 a = tip.position + tip.right * 0.5f * data.width;
        Vector3 b = tip.position - tip.right * 0.5f * data.width;

        Gizmos.DrawLine(a, a + tip.up * 10);
        Gizmos.DrawLine(b, b + tip.up * 10);
    }
}
