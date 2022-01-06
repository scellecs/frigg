namespace Frigg.Editor {
    using System;
    using UnityEditor;
    using UnityEngine;

    public class InlinePropertyDrawer : FriggPropertyDrawer {
        private float height;
        private Rect  cachedRect;

        public InlinePropertyDrawer(FriggProperty prop) : base(prop) {
        }
        
        public override void DrawLayout() {
            this.property.IsExpanded = true;

            void DrawElement(FriggProperty p) {
                p.Draw();
            }
            
            if (this.drawLayoutAction == null) {
                this.drawLayoutAction = DrawElement;
            }

            this.property.ChildrenProperties.RecurseChildren(this.drawLayoutAction);
            this.property.CallNextDrawer();
        }

        public override void Draw(Rect rect) {
            this.property.IsExpanded = true;
            this.cachedRect          = rect;
            
            void DrawElement(FriggProperty p) {
                var h = EditorGUIUtility.singleLineHeight;
                p.Draw(this.cachedRect); 
                this.cachedRect.y += h + GuiUtilities.SPACE;
            }
            
            if (this.drawAction == null) {
                this.drawAction = DrawElement;
            }

            this.property.ChildrenProperties.RecurseChildren(this.drawAction);
            this.property.CallNextDrawer(this.cachedRect);
        }

        public override float GetHeight() {
            this.height = 0f;

            void CalculateHeight(FriggProperty p) {
                if (!p.IsExpanded)
                    this.height += EditorGUIUtility.singleLineHeight;
                else
                    this.height += FriggProperty.GetPropertyHeight(p);
            }
            
            if (this.getHeightAction == null) {
                this.getHeightAction = CalculateHeight;
            }

            this.property.ChildrenProperties.RecurseChildren(this.getHeightAction);
            return this.height;
        }

        public override bool IsVisible => true;
    }
}