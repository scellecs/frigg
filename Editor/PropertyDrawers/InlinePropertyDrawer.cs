namespace Frigg.Editor {
    using System;
    using UnityEditor;
    using UnityEngine;

    public class InlinePropertyDrawer : FriggPropertyDrawer {
        public InlinePropertyDrawer(FriggProperty prop) : base(prop) {
        }
        public override void DrawLayout() {
            this.property.IsExpanded = true;
            foreach (var p in this.property.ChildrenProperties.RecurseChildren()) {
                p.Draw();
            }
        }

        public override void Draw(Rect rect) {
            this.property.IsExpanded = true;
            foreach (var p in this.property.ChildrenProperties.RecurseChildren()) {
                p.Draw(rect);
                var h = FriggProperty.GetPropertyHeight(p);
                rect.y      += h + GuiUtilities.SPACE / 2f;
                rect.height =  h + GuiUtilities.SPACE / 2f;
            }
        }

        public override float GetHeight() => FriggProperty.GetPropertyHeight(this.property);

        public override bool IsVisible => true;
        
    }
}