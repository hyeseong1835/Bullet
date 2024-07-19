using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Weapon
{
    static Player player => Player.instance;
    
    public Transform tip;

    public GameObject laserPrefab;
    GameObject laser;
    [SerializeField] float laserShowTime = 0.1f;
    public float damage;
    public float width;
    protected void Start()
    {
        laser = Instantiate(laserPrefab);
        laser.SetActive(false);
    }
    protected void OnDestroy()
    {
        Destroy(laser);
    }
    protected void Update()
    {
        transform.rotation = player.input.toMouseRot;
    }
    public override void Use()
    {
        RaycastHit2D[] hit = Physics2D.BoxCastAll(
            tip.position,
            new Vector2(width, 0.1f),
            tip.eulerAngles.z,
            tip.up,
            10,
            LayerMask.GetMask("Enemy")
        );

        foreach(RaycastHit2D info in hit)
        {
            Enemy enemy = info.collider.GetComponent<Enemy>();
            enemy.TakeDamage(damage * Player.instance.damage);
        }
        
        StartCoroutine(Grafic());
    }
    IEnumerator Grafic()
    {
        laser.SetActive(true);
        laser.transform.position = tip.position;
        laser.transform.rotation = player.input.toMouseRot;
        
        yield return new WaitForSeconds(laserShowTime);
        laser?.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 a = tip.position + tip.right * 0.5f * width;
        Vector3 b = tip.position - tip.right * 0.5f * width;
        Gizmos.DrawLine(a, a + tip.up * 10);
        Gizmos.DrawLine(b, b + tip.up * 10);
    }
}
