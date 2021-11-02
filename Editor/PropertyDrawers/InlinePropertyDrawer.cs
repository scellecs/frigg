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
        }

        public override void Draw(Rect rect) {
            this.property.IsExpanded =  true;
            foreach (var p in this.property.ChildrenProperties.RecurseChildren()) {
                //EditorGUI.BeginChangeCheck();
                var h = FriggProperty.GetPropertyHeight(p);
                    p.Draw(rect); 
                    rect.y     += h + GuiUtilities.SPACE;
                    
                /*if (EditorGUI.EndChangeCheck()) {
                    CoreUtilities.OnValueChanged(p);
                }*/
            }
        }

        public override float GetHeight() => FriggProperty.GetPropertyHeight(this.property);

        public override bool IsVisible => true;
        
    }
}