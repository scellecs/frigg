namespace Frigg.Editor.BuiltIn {
    using UnityEditor;
    using UnityEngine;

    public static class CurveDrawer {
        public static void DrawLayout(FriggProperty property) {
            var value  = DrawerUtils.GetTargetValue<AnimationCurve>(property);
            var result = EditorGUILayout.CurveField(property.Label, value);
            DrawerUtils.UpdateAndCallNext(property, result);
        }

        public static void Draw(FriggProperty property, Rect rect) {
            var value  = DrawerUtils.GetTargetValue<AnimationCurve>(property);
            var result = EditorGUI.CurveField(rect, property.Label, value);
            DrawerUtils.UpdateAndCallNext(property, result, rect);
        }

        public static float GetHeight() => EditorGUIUtility.singleLineHeight;
    }
}