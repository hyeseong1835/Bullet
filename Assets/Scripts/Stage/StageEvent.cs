using System;
using System.Reflection;
using UnityEngine;

[Serializable]
public class StageEvent
{
    public float time;
    public string typeName;
    public string methodName;

    public void Invoke()
    {
        Type type = Type.GetType(typeName);
        if(type == null)
        {
            Debug.LogError($"Type({typeName}) not found");
            return;
        }
        
        MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public);
        if (methodInfo == null)
        {
            Debug.LogError($"Method({typeName}/{methodInfo}) not found");
            return;
        }

        methodInfo.Invoke(null, null);
    }
}