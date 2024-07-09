using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(SerializableGameObjectDoubleArray))]
public class SerializableGameObjectDoubleArrayDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        CustomGUILayout.TitleHeaderLabel(label.text);

        SerializedProperty arrays = property.FindPropertyRelative("arrays");

        EditorGUI.BeginProperty(position, label, property);

        if(arrays.IsUnityNull())
        {
            CustomGUILayout.WarningLabel("Null");
        }
        else
        {
            CustomGUILayout.BeginNewTab();
            {
                for (int arrayIndex = 0; arrayIndex < arrays.arraySize; arrayIndex++)
                {
                    SerializedProperty array = arrays.GetArrayElementAtIndex(arrayIndex);
                    CustomGUILayout.TitleHeaderLabel($"{arrayIndex}");
                    for (int elementIndex = 0; elementIndex < array.arraySize; elementIndex++)
                    {
                        GameObject element = ((SerializableGameObjectArray)array).array[elementIndex];
                        EditorGUI.PropertyField(position, element, GUIContent.none);
                    }
                }
            }
            CustomGUILayout.EndNewTab();
        }

        EditorGUI.EndProperty();
    }
}
