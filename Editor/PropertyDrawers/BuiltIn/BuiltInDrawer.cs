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
                
                this.property.UpdateValue(value);

                if (!Application.isPlaying) {
                    if (this.property.NativeProperty != null) {
                        var prop             = this.property.NativeProperty;
                        var serializedObject = prop.serializedObject;
                        CoreUtilities.SetSerializedPropertyValue(prop, this.drawerType, value);
                        
                        Undo.RegisterCompleteObjectUndo(serializedObject.targetObject,
                            $"'update {prop.name} value to {value}'");
                        Undo.FlushUndoRecordObjects();
                        
                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }
            this.CallNext(rect);
        }

        protected void CallNext(Rect rect = default) {
            this.property.CallNextDrawer(rect);
        }
    }
}