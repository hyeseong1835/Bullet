using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolHolder : MonoBehaviour
{
    public static PoolHolder instance;

    void Awake() => instance = this;
}
