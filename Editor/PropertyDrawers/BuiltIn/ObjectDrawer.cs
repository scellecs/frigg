namespace Frigg.Editor.BuiltIn {
    using UnityEditor;
    using UnityEngine;

    public static class ObjectDrawer {
        public static void DrawLayout(FriggProperty property) {
            var value  = DrawerUtils.GetTargetValue<Object>(property);
            var result = EditorGUILayout.ObjectField(property.Label, value, property.MetaInfo.MemberType, true);
            DrawerUtils.UpdateAndCallNext(property, result);
        }

        public static void Draw(FriggProperty property, Rect rect) {
            var value  = DrawerUtils.GetTargetValue<Object>(property);
            var result = EditorGUI.ObjectField(rect, property.Label, value, property.MetaInfo.MemberType, true);
            rect.y += EditorGUIUtility.singleLineHeight;
            DrawerUtils.UpdateAndCallNext(property, result, rect);
        }

        public static float GetHeight() => EditorGUIUtility.singleLineHeight;
    }
}