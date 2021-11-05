namespace Frigg.Editor.BuiltIn {
    using UnityEditor;
    using UnityEngine;

    public static class GradientDrawer {
        public static void DrawLayout(FriggProperty property) {
            var value  = DrawerUtils.GetTargetValue<Gradient>(property);
            var result = EditorGUILayout.GradientField(property.Label, value);
            DrawerUtils.UpdateAndCallNext(property, result);
        }

        public static void Draw(FriggProperty property, Rect rect) {
            var value  = DrawerUtils.GetTargetValue<Gradient>(property);
            var result = EditorGUI.GradientField(rect, property.Label, value);
            DrawerUtils.UpdateAndCallNext(property, result, rect);
        }

        public static float GetHeight() => 0;
    }
}