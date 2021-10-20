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

                //Update "object" value
                this.property.Update(value);

                if (!string.IsNullOrEmpty(this.property.UnityPath)) {
                    var serializedObject = this.property.PropertyTree.SerializedObject;
                    var prop             = serializedObject.FindProperty(this.property.UnityPath);

                    //Save changes inside target SerializedObject.
                    CoreUtilities.SetSerializedPropertyValue(prop, this.drawerType, value);
                    EditorUtility.SetDirty(serializedObject.targetObject);
                    serializedObject.ApplyModifiedProperties();
                }
            }
            this.CallNext(rect);
        }

        protected void CallNext(Rect rect = default) {
            this.property.CallNextDrawer(rect);
        }
    }
}