namespace Frigg.Editor {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using BuiltIn;
    using Layouts;
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public class FoldoutPropertyDrawer : FriggPropertyDrawer {
        public FoldoutPropertyDrawer(FriggProperty prop) : base(prop) {
        }
        
        public override void DrawLayout() {
            this.property.IsExpanded = GuiUtilities.FoldoutToggle(this.property);
            
            if (!this.property.IsExpanded) {
                this.property.CallNextDrawer();
                return;
            }

            EditorGUI.indentLevel++;
            
            foreach (var p in this.property.ChildrenProperties.RecurseChildren()) {
                p.Draw();
            }

            EditorGUI.indentLevel--;
            this.property.CallNextDrawer();
        }

        public override void Draw(Rect rect) {
            /*Algorithm: List -> Foldout -> list of foldouts.
             *Start drawing foldout with Toggle (18f), then if expanded - for each element draw spacing (5f) + element (element height)
             * 
             */
            this.property.PropertyTree.LayoutsByPath.TryGetValue
                (this.property.Path, out var layout);
            
            if (layout != null) {
                layout.Draw(rect);
                return;
            }

            //Foldout toggle
            this.property.IsExpanded = GuiUtilities.FoldoutToggle(this.property, rect);
            
            //If !expanded - skip all the next logic and move to the next drawer.
            if (!this.property.IsExpanded) {
                this.property.CallNextDrawer(rect);
                return;
            }
            
            //Add 18f as height because of foldout toggle.
            rect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.indentLevel++;

            var prevProp = this.property;
            foreach (var p in this.property.ChildrenProperties.RecurseChildren()) {
                //Padding in the beginning of an each element.
                if (p.TryGetFixedAttribute<DisplayAsString>() != null) {
                    prevProp =  p;
                    continue;
                }
                
                if (prevProp.TryGetFixedAttribute<DisplayAsString>() != null) {
                    p.Label.text = (string) prevProp.GetValue();
                }

                rect.y += GuiUtilities.SPACE;
                p.Draw(rect);

                var h = FriggProperty.GetPropertyHeight(p);
                rect.y      += h - GuiUtilities.SPACE;
                rect.height =  EditorGUIUtility.singleLineHeight;
            }
            
            EditorGUI.indentLevel--;
            this.property.CallNextDrawer(rect);
        }

        //if property has FoldoutDrawer - then add 18F if !expanded or calculate all other drawers if expanded
        public override float GetHeight() {
            //foldout header
            var height = EditorGUIUtility.singleLineHeight;

            if (!this.property.IsExpanded) {
                return height;
            }
            
            //Get all properties that were stored in our foldout
            var properties = this.property.ChildrenProperties.RecurseChildren();

            foreach (var prop in properties) {
                //var propHeight = FriggProperty.GetPropertyHeight(prop);
                height += FriggProperty.GetPropertyHeight(prop);
            }
            
            //Debug.Log(height);
            return height;
        }

        public override bool IsVisible => true;
    }
}