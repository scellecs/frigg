namespace Frigg.Editor {
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public class ButtonDrawer : BaseDrawer {
        public ButtonDrawer(FriggProperty prop) : base(prop) {
        }

        public override void DrawLayout() {
            if(GUILayout.Button(this.property.Label)) {
                var name   = this.property.Name;
                var method = this.property.ParentProperty.GetValue().TryGetMethod(name);
                
                method.Invoke(this.property.ParentProperty.GetValue(), null);
            };
            
            this.property.CallNextDrawer();
        }

        public override void Draw(Rect rect) { 
            if(GUI.Button(rect, this.property.Label)) {
                var name   = this.property.Name;
                var method = this.property.ParentProperty.GetValue().TryGetMethod(name);
                
                method.Invoke(this.property.ParentProperty.GetValue(), null);
                rect.y += 25;
            };
            
            this.property.CallNextDrawer(rect);
        }

        public override float GetHeight() => 25f;

        public override bool IsVisible => true;
    }
}