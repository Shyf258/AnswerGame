//2018.08.30    关林
//自定义绘制

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(GL_NameAttribute))]
public class GL_NameAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label.text = ((GL_NameAttribute)attribute)._name;
        EditorGUI.PropertyField(position, property, label);
    }
}
