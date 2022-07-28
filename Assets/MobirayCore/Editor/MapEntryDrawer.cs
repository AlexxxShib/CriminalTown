using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MobirayCore.Helpers.Editor
{
    //[CustomPropertyDrawer(typeof(HashMap<,>.MapEntry))]
    public class MapEntryDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            
            var keyField = new PropertyField(property.FindPropertyRelative("key"));
            var valueField = new PropertyField(property.FindPropertyRelative("value"));

            container.Add(keyField);
            container.Add(valueField);

            return container;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var kexpanded = property.FindPropertyRelative("key").isExpanded;
            property.FindPropertyRelative("key").isExpanded = true;
            var kwd = property.FindPropertyRelative("key").CountInProperty() > 1 ? 15 : 0;
            property.FindPropertyRelative("key").isExpanded = kexpanded;
            
            var vexpanded = property.FindPropertyRelative("value").isExpanded;
            property.FindPropertyRelative("value").isExpanded = true;
            var vwd = property.FindPropertyRelative("value").CountInProperty() > 1 ? 15 : 0;
            property.FindPropertyRelative("value").isExpanded = vexpanded;

            // Calculate rects
            var keyRect = new Rect(position.x + kwd, position.y, position.width / 2 - 1 - kwd, position.height);
            var valueRect = new Rect(position.x + position.width / 2 + 1 + vwd, position.y, position.width / 2 - 1 - vwd, position.height);

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(keyRect, property.FindPropertyRelative("key"), GUIContent.none, true);
            EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none, true);
 
            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return Mathf.Max(EditorGUI.GetPropertyHeight(property, label, true), 
                EditorGUI.GetPropertyHeight(property.FindPropertyRelative("value"), GUIContent.none, true));
        }
    }
}
