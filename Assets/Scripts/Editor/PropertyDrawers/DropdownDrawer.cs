namespace Assets.Scripts.Editor.PropertyDrawers {
    using System;
    using System.Collections;
    using Attributes;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    [CustomPropertyDrawer(typeof(DropdownAttribute))]
    public class DropdownDrawer : BaseDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            var attr = (DropdownAttribute) this.attribute;

            var target = property.serializedObject.targetObject;

            var values = this.GetDropdownValues(property, attr.Name);

            if (values is IList list) {
                
                var currValue = this.fieldInfo.GetValue(target);

                var size = list.Count;
                
                var valuesArr = new object[size];
                var options   = new string[size];

                for (var i = 0; i < size; i++) {
                    valuesArr[i] = list[i];
                    options[i]   = list[i].ToString();
                }

                var currIndex = Array.IndexOf(valuesArr, currValue);
            
                GuiUtilities.Dropdown(position, property.serializedObject, target, 
                    fieldInfo, label.text, currIndex, options, valuesArr);
            }
            
            EditorGUI.EndProperty();
        }

        private object GetDropdownValues(SerializedProperty prop, string name) {
            var target = prop.serializedObject.targetObject;

            var fInfo = target.TryGetField(name);

            if (fInfo != null) {
                return fInfo.GetValue(target);
            }

            Debug.LogError($"There aren't any field with name {name}");
            return null;
        }
    }
}