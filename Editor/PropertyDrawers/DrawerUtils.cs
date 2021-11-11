namespace Frigg.Editor {
    using System;
    using Packages.Frigg.Editor.Utils;
    using UnityEngine;
    using UnityEditor;
    using Utils;

    public static class DrawerUtils {
        public static T GetTargetValue<T>(FriggProperty property) {
            EditorGUI.BeginChangeCheck();
            return (T)property.GetValue();
        }

        public static void UpdateAndCallNext<T>(FriggProperty property, T value, Rect rect = default) =>
            UpdateAndCallNext(typeof(T), property, value, rect);

        public static void UpdateAndCallNext(Type drawerType, FriggProperty property, object value, Rect rect = default) {
            if (EditorGUI.EndChangeCheck()) {
                property.UpdateValue(value);
                CoreUtilities.OnValueChanged(property);

                if (!Application.isPlaying) {
                    if (property.NativeProperty != null) {
                        var prop = property.NativeProperty;
                        var serializedObject = prop.serializedObject;
                        CoreUtilities.SetSerializedPropertyValue(prop, drawerType, value);

                        Undo.RegisterCompleteObjectUndo(serializedObject.targetObject,
                            $"'update {prop.name} value to {value}'");
                        Undo.FlushUndoRecordObjects();

                        serializedObject.ApplyModifiedProperties();
                    }
                }
            }
            CallNext(property, rect);
        }

        public static void CallNext(FriggProperty property, Rect rect = default) {
            property.CallNextDrawer(rect);
        }
    }
}