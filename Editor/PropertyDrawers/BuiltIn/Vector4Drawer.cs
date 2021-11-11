namespace Frigg.Editor.BuiltIn {
    using UnityEditor;
    using UnityEngine;
    using Vector4 = UnityEngine.Vector4;

    public static class Vector4Drawer {
        public static void DrawLayout(FriggProperty property) {
            var value  = DrawerUtils.GetTargetValue<Vector4>(property);
            var result = EditorGUILayout.Vector4Field(property.Label, value);
            DrawerUtils.UpdateAndCallNext(property, result);
        }

        public static void Draw(FriggProperty property, Rect rect) {
            var value  = DrawerUtils.GetTargetValue<Vector4>(property);
            var result = EditorGUI.Vector4Field(rect, property.Label, value);
            rect.y += EditorGUIUtility.singleLineHeight;
            DrawerUtils.UpdateAndCallNext(property, result, rect);
        }

        public static float GetHeight() => EditorGUIUtility.singleLineHeight;
    }
}