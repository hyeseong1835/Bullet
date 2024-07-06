using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolHolder : MonoBehaviour
{
    public static PoolHolder instance;
    public List<Pool> pools = new List<Pool>();
    public List<Pool> enemyPools = new List<Pool>();
    public List<Pool> bulletPools = new List<Pool>();
    public List<Pool> itemPools = new List<Pool>();

    void Awake() => instance = this;
    void OnValidate() => instance = this;
}
