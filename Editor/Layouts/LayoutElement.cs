namespace Frigg.Editor.Layouts {
    using System.Drawing;
    using Groups;
    using UnityEngine;

    public class LayoutElement {
        public readonly FriggProperty      property;
        public readonly BaseGroupAttribute attribute;

        public float height;
        public float width;
        
        public LayoutElement(FriggProperty property, BaseGroupAttribute attribute) {
            this.property  = property;
            this.attribute = attribute;
        }
    }
}