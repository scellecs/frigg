namespace Assets.Scripts.Editor.PropertyDrawers {
    using System;
    using Attributes;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsDrawer : BaseDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            var attr   = (EnumFlagsAttribute) this.attribute;
            var target = (Enum)CoreUtilities.GetTargetObjectOfProperty(property);
            
            var lab    = string.IsNullOrEmpty(attr.Name) ? label.text : attr.Name;

            if (target == null) {
                Debug.LogError("Invalid target.");
                return;
            }

            var enumValues = EditorGUI.EnumFlagsField(position, lab, target);

            property.intValue = (int)Convert.ChangeType(enumValues, target.GetType());

            property.serializedObject.ApplyModifiedProperties(); //TODO: Callback 

            EditorGUI.EndProperty();
        }
    }
}