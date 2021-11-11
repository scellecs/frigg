namespace Frigg.Editor.BuiltIn {
    using UnityEditor;
    using UnityEngine;

    public static class BoolDrawer {
        public static void DrawLayout(FriggProperty property) {
            var value = DrawerUtils.GetTargetValue<bool>(property);
            var result = EditorGUILayout.Toggle(property.Label, value);
            DrawerUtils.UpdateAndCallNext(property, result);
        }

        public static void Draw(FriggProperty property, Rect rect) {
            var value  = DrawerUtils.GetTargetValue<bool>(property);
            var result = EditorGUI.Toggle(rect, property.Label, value);
            rect.y += EditorGUIUtility.singleLineHeight;
            DrawerUtils.UpdateAndCallNext(property, result, rect);
        }

        public static float GetHeight() => EditorGUIUtility.singleLineHeight;
    }
}