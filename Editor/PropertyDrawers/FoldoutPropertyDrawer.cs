namespace Frigg.Editor {
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    public class FoldoutPropertyDrawer : FriggPropertyDrawer {
        public FoldoutPropertyDrawer(FriggProperty prop) : base(prop) {
        }
        
        public override void DrawLayout() {
            var style = EditorStyles.foldoutHeader;
            this.property.IsExpanded = GUILayout.Toggle(this.property.IsExpanded, this.property.Label, style);
            
            if (!this.property.IsExpanded) {
                return;
            }

            foreach (var p in this.property.ChildrenProperties.RecurseChildren()) {
                p.Draw();
            }
        }

        public override void Draw(Rect rect) {
            
            //To editor tools
            var style = EditorStyles.foldoutHeader;

            var toggleRect = rect;
            var indent = EditorGUI.indentLevel * 15;
            toggleRect.width         -= indent;
            toggleRect.x             += indent;
            this.property.IsExpanded =  GUI.Toggle(toggleRect, this.property.IsExpanded, this.property.Label, style);
            
            if (!this.property.IsExpanded) {
                return;
            }

            rect.y      += EditorGUIUtility.singleLineHeight;

            EditorGUI.indentLevel++;
            foreach (var p in this.property.ChildrenProperties.RecurseChildren()) {
                p.Draw(rect);
                var h = FriggProperty.GetPropertyHeight(p);
                rect.y      += h + GuiUtilities.SPACE / 2;
                rect.height =  EditorGUIUtility.singleLineHeight;
            }
            EditorGUI.indentLevel--;
        }

        public override float GetHeight() {
            return FriggProperty.GetPropertyHeight(this.property);
        }

        public override bool IsVisible => true;
    }
}