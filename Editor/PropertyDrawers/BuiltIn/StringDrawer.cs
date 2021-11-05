namespace Frigg.Editor.BuiltIn {
    using UnityEditor;
    using UnityEngine;

    public static class StringDrawer {        
        public static void DrawLayout(FriggProperty property) {
            var value  = DrawerUtils.GetTargetValue<string>(property);
            var result = EditorGUILayout.TextField(property.Label, value);
            DrawerUtils.UpdateAndCallNext(property, result);
        }

        public static void Draw(FriggProperty property, Rect rect) {
            var value = DrawerUtils.GetTargetValue<string>(property);
            var result = EditorGUI.TextField(rect, property.Label, value);
            rect.y += EditorGUIUtility.singleLineHeight;
            DrawerUtils.UpdateAndCallNext(property, result, rect);
        }

        public static float GetHeight() => EditorGUIUtility.singleLineHeight;
    }

    public static class LabelStringDrawer {
        public static void DrawLayout(FriggProperty property) {
            var value = DrawerUtils.GetTargetValue<string>(property);

            EditorGUILayout.LabelField(value);
            DrawerUtils.CallNext(property);
        }

        public static void Draw(FriggProperty property, Rect rect) {
            var value = DrawerUtils.GetTargetValue<string>(property);

            EditorGUI.LabelField(rect, value);
            DrawerUtils.CallNext(property, rect);            
        }

        public static float GetHeight() => GuiUtilities.SPACE;
    }
}