namespace Frigg.Editor {
    using Groups;
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using UnityEngine;

    public class HorizontalGroupDrawer : BaseGroupDrawer {
        private HorizontalGroupAttribute attribute;
        public HorizontalGroupDrawer(FriggProperty prop) : base(prop) {
            this.linkedAttribute = prop.TryGetFixedAttribute<HorizontalGroupAttribute>();
            var attr = (HorizontalGroupAttribute) this.linkedAttribute;
            GroupsHandler.AddGroup(new GroupInfo(attr.GroupName, prop.ParentProperty.Path));
        }
        
        public override void DrawLayout() {
        }

        public override void Draw(Rect rect) {
            this.attribute =  (HorizontalGroupAttribute) this.linkedAttribute;
            this.property.CallNextDrawer(rect);
        }

        public override float GetHeight() => 0;

        public override bool IsVisible => true;
    }
}