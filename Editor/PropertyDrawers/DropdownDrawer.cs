namespace Frigg.Editor {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using Utils;
    
    public class DropdownDrawer : BaseDrawer {
        public DropdownDrawer(FriggProperty prop) : base(prop) {
        }

        public override void DrawLayout() {
            this.DoDropdown();
        }

        public override void Draw(Rect rect) {
            this.DoDropdown(rect);
            
            rect.y += EditorGUIUtility.singleLineHeight;
            this.property.CallNextDrawer(rect);
        }

        private void DoDropdown(Rect position = default) {
            var attr          = (DropdownAttribute) this.linkedAttribute;
            var target        = this.property.ParentValue;
            var values        = this.GetDropdownValues(this.property, attr.Name);
            var selectedValue = this.property.PropertyValue.Value;

            if (position == Rect.zero) {
                position = EditorGUILayout.GetControlRect(true);
            }

            switch (values) {
                case IList list: {
                    var currValue = this.property.PropertyValue.Value;

                    var size = list.Count;
                
                    var valuesArr = new object[size];
                    var options   = new GUIContent[size];

                    for (var i = 0; i < size; i++) {
                        valuesArr[i]    = list[i];
                        options[i] = new GUIContent(list[i].ToString());
                    }

                    var currIndex = Array.IndexOf(valuesArr, currValue);
                    
                    GuiUtilities.Dropdown(position, property.PropertyTree.SerializedObject, target, 
                        (FieldInfo) this.property.MetaInfo.MemberInfo, this.property.Label, currIndex, options, valuesArr);
                    break;
                }
                
                case IDropdownList droplist: {
                    var val     = new List<object>();
                    var options = new List<GUIContent>();

                    var currIndex = 0;
                    var selected  = 0;

                    using (var enumerator = droplist.GetEnumerator()) {

                        while (enumerator.MoveNext()) {
                            var current = enumerator.Current;

                            if (current.Value.Equals(selectedValue)) {
                                selected = currIndex;
                            }

                            val.Add(current.Value);
                            options.Add(new GUIContent(current.Key));
                        
                            currIndex++;
                        }
                        GuiUtilities.Dropdown(position, this.property.PropertyTree.SerializedObject, target, 
                            (FieldInfo) this.property.MetaInfo.MemberInfo, this.property.Label, selected, options.ToArray(), val.ToArray());
                    }

                    break;
                }
            }
        }

        public override float GetHeight() => EditorGUIUtility.singleLineHeight;

        public override bool IsVisible => true;

        private object GetDropdownValues(FriggProperty prop, string name) {
            var target = prop.ParentValue;

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