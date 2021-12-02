namespace Frigg.Editor {
    using System;
    using Groups;
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public class InlinePropertyDrawer : FriggPropertyDrawer {
        public InlinePropertyDrawer(FriggProperty prop) : base(prop) {
        }
        public override void DrawLayout() {
            this.property.IsExpanded = true;
            foreach (var p in this.property.ChildrenProperties.RecurseChildren()) {
                p.Draw();
            }
            
            this.property.CallNextDrawer();
        }

        public override void Draw(Rect rect) {
            this.property.IsExpanded =  true;
            foreach (var p in this.property.ChildrenProperties.RecurseChildren()) {
                var h = EditorGUIUtility.singleLineHeight;
                    p.Draw(rect); 
                    rect.y += h + GuiUtilities.SPACE;
            }
            
            this.property.CallNextDrawer(rect);
        }

        public override float GetHeight() {
            var height = 0f;
            foreach (var p in this.property.ChildrenProperties.RecurseChildren()) {
                height += FriggProperty.GetPropertyHeight(p);
            }

            return height;
        }

        public override bool IsVisible => true;
        
    }
}