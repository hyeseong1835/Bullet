using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class SerializableGameObjectArray
{
    public GameObject[] array;

    public static implicit operator GameObject[](SerializableGameObjectArray array) => array.array;
    public static implicit operator SerializableGameObjectArray(GameObject[] array) => new SerializableGameObjectArray(array);

    public SerializableGameObjectArray(GameObject[] array)
    {
        this.array = array;
    }
}
[System.Serializable]
public class SerializableGameObjectDoubleArray
{
    public SerializableGameObjectArray[] arrays;

    public SerializableGameObjectDoubleArray(GameObject[][] arrays)
    {
        foreach (GameObject[] array in arrays)
        {
            this.arrays = new SerializableGameObjectArray[arrays.Length];
            for (int i = 0; i < arrays.Length; i++)
            {
                this.arrays[i] = new SerializableGameObjectArray(arrays[i]);
            }
        }
    }
}