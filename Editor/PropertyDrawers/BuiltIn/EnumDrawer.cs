namespace Frigg.Editor.BuiltIn {
    using System;
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public static class EnumDrawer {
        public static void DrawLayout(FriggProperty property) {
            DoEnumField(property);
        }

        public static void Draw(FriggProperty property, Rect rect) {
            DoEnumField(property, rect);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        private static void DoEnumField(FriggProperty property, Rect rect = default) {
            EditorGUI.BeginChangeCheck();
            var target = (Enum) CoreUtilities.GetTargetValue(property.ParentProperty.GetValue(), property.MetaInfo.MemberInfo);

            if (target == null) {
                Debug.LogError("Invalid target.");
                return;
            }
            
            var enumValue = rect == default 
                ? EditorGUILayout.EnumPopup(property.Label, target)
                : EditorGUI.EnumPopup(rect, property.Label, target);

            if (rect != default) {
                rect.y += EditorGUIUtility.singleLineHeight;
            }
            DrawerUtils.UpdateAndCallNext(property, enumValue, rect);
        }

        public static float GetHeight() => EditorGUIUtility.singleLineHeight;
    }
}