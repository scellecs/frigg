namespace Frigg.Editor {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using BuiltIn;
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using UnityEngine;
    using Utils;

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
            var layout = this.property.PropertyTree.Layouts.
                FirstOrDefault(x => x.layoutPath == this.property.Path);
            
            if (layout != null) {
                layout.Draw(rect);
                return;
            }

            this.property.IsExpanded = GuiUtilities.FoldoutToggle(this.property, rect);
            
            if (!this.property.IsExpanded) {
                return;
            }

            rect.y += EditorGUIUtility.singleLineHeight + GuiUtilities.SPACE;

            EditorGUI.indentLevel++;

            var prevProp = this.property;
            foreach (var p in this.property.ChildrenProperties.RecurseChildren()) {
                if (p.TryGetFixedAttribute<DisplayAsString>() != null) {
                    rect.y   += GuiUtilities.SPACE;
                    prevProp =  p;
                    continue;
                }
                
                if (prevProp.TryGetFixedAttribute<DisplayAsString>() != null) {
                    p.Label.text = (string) prevProp.GetValue();
                }

                p.Draw(rect);

                var h = FriggProperty.GetPropertyHeight(p);
                rect.y      += h;
                rect.height =  EditorGUIUtility.singleLineHeight;
            }
            
            EditorGUI.indentLevel--;
        }

        //if property has FoldoutDrawer - then add 18F if !expanded or calculate all other drawers if expanded
        public override float GetHeight() {
            var height     = EditorGUIUtility.singleLineHeight;

            if (!this.property.IsExpanded) {
                return height;
            }
            
            //Get all properties that were stored in our foldout
            var properties = this.property.ChildrenProperties.RecurseChildren();

            foreach (var prop in properties) {
                height += FriggProperty.GetPropertyHeight(prop);
            }

            return height;
        }

        public override bool IsVisible => true;
    }
}