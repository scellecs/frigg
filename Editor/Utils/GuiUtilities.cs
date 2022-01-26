namespace Frigg.Editor {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using Utils;
    using Object = UnityEngine.Object;

    public static class GuiUtilities {
        public const   float SPACE              = 5.0f;
        private const  int   PROPERTY_MIN_WIDTH = 212;

        public static void DrawTree(PropertyTree tree) {
            tree.Draw();
        }

        public static bool FoldoutToggle(FriggProperty property, Rect rect = default) {
            var style = EditorStyles.foldoutHeader;

            if (rect == Rect.zero) {
                rect = EditorGUILayout.GetControlRect(true);
            }

            var toggleRect = rect;
            var indent     = EditorGUI.indentLevel * 15;
            
            toggleRect.width -= indent;
            toggleRect.x     += indent;
            
            var color = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.8f,0.8f,0.8f);
            
            //Each frame disabled group
            EditorGUI.EndDisabledGroup();

            //Readonly disabled group
            if (property.IsReadonly) {
                EditorGUI.EndDisabledGroup();
            }

            var value = GUI.Toggle(toggleRect, property.IsExpanded, property.Label, style);
            
            EditorGUI.BeginDisabledGroup(property.IsReadonly);
            
            EditorData.SetBoolValue(property.Path, value);
            property.IsExpanded = value;
            
            GUI.backgroundColor = color;
            return value;
        }
    }
}