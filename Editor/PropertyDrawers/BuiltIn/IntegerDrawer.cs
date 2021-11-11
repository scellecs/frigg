namespace Frigg.Editor.BuiltIn {
    using UnityEditor;
    using UnityEngine;

    public static class IntegerDrawer{
        public static void DrawLayout(FriggProperty property) {
            var value  = DrawerUtils.GetTargetValue<int>(property);
            var result = EditorGUILayout.IntField(property.Label, value);
            DrawerUtils.UpdateAndCallNext(property, result);
        }

        public static void Draw(FriggProperty property, Rect rect) {
            var value  = DrawerUtils.GetTargetValue<int>(property);
            var result = EditorGUI.IntField(rect, property.Label, value);
            rect.y += EditorGUIUtility.singleLineHeight;
            DrawerUtils.UpdateAndCallNext(property, result, rect);
        }

        public static float GetHeight() => EditorGUIUtility.singleLineHeight;
    }
}