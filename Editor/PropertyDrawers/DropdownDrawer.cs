﻿namespace Assets.Scripts.Editor.PropertyDrawers {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Attributes;
    using UnityEditor;
    using UnityEngine;
    using Utils;
    using Object = UnityEngine.Object;

    [CustomPropertyDrawer(typeof(DropdownAttribute))]
    public class DropdownDrawer : BaseDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            EditorGUI.BeginProperty(position, new GUIContent(property.displayName), property);
            
            var attr          = (DropdownAttribute) this.attribute;
            var target        = property.serializedObject.targetObject;
            var values        = this.GetDropdownValues(property, attr.Name);
            var selectedValue = fieldInfo.GetValue(target);

            switch (values) {
                case IList list: {
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
                        this.fieldInfo, property.displayName, currIndex, options, valuesArr);
                    break;
                }
                
                case IDropdownList droplist: {
                    var val     = new List<object>();
                    var options = new List<string>();

                    var currIndex = 0;
                    var selected  = 0;

                    using (var enumerator = droplist.GetEnumerator()) {

                        while (enumerator.MoveNext()) {
                            var current = enumerator.Current;

                            if (current.Value.Equals(selectedValue)) {
                                selected = currIndex;
                            }

                            val.Add(current.Value);
                            options.Add(current.Key);
                        
                            currIndex++;
                        }

                        GuiUtilities.Dropdown(position, property.serializedObject, target, 
                            this.fieldInfo, property.displayName, selected, options.ToArray(), val.ToArray());
                    }

                    break;
                }
            }
            
            EditorGUI.EndProperty();
        }

        private object GetDropdownValues(SerializedProperty prop, string name) {
            var target = prop.serializedObject.targetObject;

            var fInfo = target.TryGetField(name);
            if (fInfo != null) {
                return fInfo.GetValue(target);
            }
            
            var mInfo = target.TryGetMethod(name);
            if (mInfo != null && mInfo.ReturnType != typeof(void)) {
                return mInfo.Invoke(target, null);
            }

            var pInfo = target.TryGetProperty(name);
            if (pInfo != null) {
                return pInfo.GetValue(target);
            }

            Debug.LogError($"There aren't any field with name {name}");
            return null;
        }
    }
}