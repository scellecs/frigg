namespace Frigg.Editor.BuiltIn {
    using UnityEditor;
    using UnityEngine;

    public static class LongDrawer {
        public static void DrawLayout(FriggProperty property) {
            var value  = DrawerUtils.GetTargetValue<long>(property);
            var result = EditorGUILayout.LongField(property.Label, value);
            DrawerUtils.UpdateAndCallNext(property, result);
        }

        public static void Draw(FriggProperty property, Rect rect) {
            var value  = DrawerUtils.GetTargetValue<long>(property);
            var result = EditorGUI.LongField(rect, property.Label, value);
            rect.y += EditorGUIUtility.singleLineHeight;
            DrawerUtils.UpdateAndCallNext(property, result, rect);
        }

        public static float GetHeight() => EditorGUIUtility.singleLineHeight;
    }
}