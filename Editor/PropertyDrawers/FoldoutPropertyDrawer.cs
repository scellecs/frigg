namespace Frigg.Editor {
    using System;
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using UnityEngine;

    public class FoldoutPropertyDrawer : FriggPropertyDrawer {
        private float         height;
        private Rect          cachedRect;
        private FriggProperty prevProp;

        public FoldoutPropertyDrawer(FriggProperty prop) : base(prop) {
        }
        
        public override void DrawLayout() {
            this.property.IsExpanded = GuiUtilities.FoldoutToggle(this.property);
            
            if (!this.property.IsExpanded) {
                this.property.CallNextDrawer();
                return;
            }

            EditorGUI.indentLevel++;

            if (this.drawLayoutAction == null) {
                this.drawLayoutAction = friggProperty => { friggProperty.Draw(); };
            }

            this.property.ChildrenProperties.RecurseChildren(this.drawLayoutAction);

            EditorGUI.indentLevel--;
            this.property.CallNextDrawer();
        }

        public override void Draw(Rect rect) {
            /*Algorithm: List -> Foldout -> list of foldouts.
             *Start drawing foldout with Toggle (18f), then if expanded - for each element draw spacing (5f) + element (element height)
             * 
             */

            this.cachedRect = rect;
            this.property.PropertyTree.LayoutsByPath.TryGetValue
                (this.property.Path, out var layout);
            
            if (layout != null) {
                layout.Draw(rect);
                return;
            }

            //Foldout toggle
            this.property.IsExpanded = GuiUtilities.FoldoutToggle(this.property, this.cachedRect);
            
            //If !expanded - skip all the next logic and move to the next drawer.
            if (!this.property.IsExpanded) {
                this.property.CallNextDrawer(this.cachedRect);
                return;
            }
            
            //Add 18f as height because of foldout toggle.
            this.cachedRect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.indentLevel++;

            this.prevProp = this.property;

            if (this.drawAction == null) {
                this.drawAction = (p => {
                    //Padding in the beginning of an each element.
                    if (p.TryGetFixedAttribute<DisplayAsString>() != null) {
                        this.prevProp = p;
                        return;
                    }

                    if (this.prevProp.TryGetFixedAttribute<DisplayAsString>() != null) {
                        p.Label.text = (string)this.prevProp.GetValue();
                    }

                    this.cachedRect.y += GuiUtilities.SPACE;
                    p.Draw(this.cachedRect);

                    var h = FriggProperty.GetPropertyHeight(p);
                    this.cachedRect.y      += h - GuiUtilities.SPACE;
                    this.cachedRect.height =  EditorGUIUtility.singleLineHeight;
                });
            }

            this.property.ChildrenProperties.RecurseChildren(this.drawAction);
            
            EditorGUI.indentLevel--;
            this.property.CallNextDrawer(this.cachedRect);
        }

        //if property has FoldoutDrawer - then add 18F if !expanded or calculate all other drawers if expanded
        public override float GetHeight() {
            //foldout header
            this.height = EditorGUIUtility.singleLineHeight;

            if (!this.property.IsExpanded) {
                return this.height;
            }

            if (this.getHeightAction == null) {
                this.getHeightAction = friggProperty 
                    => { this.height += FriggProperty.GetPropertyHeight(friggProperty); };
            }

            //Get all properties that were stored in our foldout
            this.property.ChildrenProperties.RecurseChildren(this.getHeightAction);
            
            return this.height;
        }

        public override bool IsVisible => true;
    }
}