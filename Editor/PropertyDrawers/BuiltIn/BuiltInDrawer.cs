namespace Frigg.Editor.BuiltIn {
    using System;
    using System.Collections;
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public abstract class BuiltInDrawer : BaseDrawer {
        private Type drawerType;
        protected BuiltInDrawer(FriggProperty prop) : base(prop) {
        }

        protected T GetTargetValue<T>() {
            this.drawerType = typeof(T);
            EditorGUI.BeginChangeCheck();
            return (T) this.property.GetValue();
        }

        protected void UpdateAndCallNext(object value, Rect rect = default) {
            if (EditorGUI.EndChangeCheck()) {
                CoreUtilities.OnValueChanged(this.property);

                this.property.Update(value);
                
                if (!string.IsNullOrEmpty(this.property.UnityPath)) {
                    var prop = this.property.PropertyTree.SerializedObject.FindProperty(this.property.UnityPath);
                    CoreUtilities.SetSerializedPropertyValue(prop, this.drawerType, value);
                    EditorUtility.SetDirty(this.property.PropertyTree.SerializedObject.targetObject);
                }
            }
            this.CallNext(rect);
        }

        protected void CallNext(Rect rect = default) {
            this.property.CallNextDrawer(rect);
        }
    }
}