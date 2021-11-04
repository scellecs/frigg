namespace Frigg.Editor {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using Packages.Frigg.Editor.Utils;
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
        }

        private void DoDropdown(Rect position = default) {
            EditorGUI.BeginChangeCheck();
            
            var attr          = (DropdownAttribute) this.linkedAttribute;
            var values        = this.GetDropdownValues(this.property, attr.Name);
            var selectedValue = this.property.GetValue();

            if (position == Rect.zero) {
                position = EditorGUILayout.GetControlRect(true);
            }

            switch (values) {
                case IList list: {
                    var currValue = this.property.GetValue();

                    var size = list.Count;
                
                    var valuesArr = new object[size];
                    var options   = new GUIContent[size];

                    for (var i = 0; i < size; i++) {
                        valuesArr[i]    = list[i];
                        options[i] = new GUIContent(list[i].ToString());
                    }

                    var currIndex = Array.IndexOf(valuesArr, currValue);

                    if (currIndex == -1) {
                        currIndex = 1;
                    }
                    
                    int newIndex;

                    if (position != default) {
                        newIndex   =  EditorGUI.Popup(position, this.property.Label, currIndex, options);
                        position.y += EditorGUIUtility.singleLineHeight;
                    }
                    else {
                        newIndex = EditorGUILayout.Popup(this.property.Label, currIndex, options);
                    }
                    
                    this.UpdateAndCallNext(valuesArr[newIndex], position);
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
                        
                        if (position != default) {
                            selected   =  EditorGUI.Popup(position, this.property.Label, selected, options.ToArray());
                            position.y += EditorGUIUtility.singleLineHeight;
                        }
                        else {
                            selected = EditorGUILayout.Popup(this.property.Label, selected, options.ToArray());
                        }
                        
                        this.UpdateAndCallNext(val[selected], position);
                    }
                    break;
                }
            }
        }

        public override float GetHeight() => EditorGUIUtility.singleLineHeight;

        public override bool IsVisible => true;

        private object GetDropdownValues(FriggProperty prop, string name) {
            var target = prop.ParentProperty.GetValue();

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