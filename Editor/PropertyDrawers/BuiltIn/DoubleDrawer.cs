namespace Frigg.Editor.BuiltIn {
    using UnityEditor;
    using UnityEngine;

    public static class DoubleDrawer {
        public static void DrawLayout(FriggProperty property) {
            var value  = DrawerUtils.GetTargetValue<double>(property);
            var result = EditorGUILayout.DoubleField(property.Label, value);
            DrawerUtils.UpdateAndCallNext(property, result);
        }

        public static void Draw(FriggProperty property, Rect rect) {
            var value  = DrawerUtils.GetTargetValue<double>(property);
            var result = EditorGUI.DoubleField(rect, property.Label, value);
            rect.y += EditorGUIUtility.singleLineHeight;
            DrawerUtils.UpdateAndCallNext(property, result, rect);
        }

        public static float GetHeight() => EditorGUIUtility.singleLineHeight;
    }
}