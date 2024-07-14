using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListUtility
{
    public static ObjT[][] ToDoubleArray<ObjT>(this List<List<ObjT>> lists)
    {
        List<ObjT[]> result = new List<ObjT[]>();
        for (int i = 0; i < lists.Count; i++)
        {
            result.Add(lists[i].ToArray());
        }
        return result.ToArray();
    }
}
