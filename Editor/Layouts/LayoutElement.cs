namespace Frigg.Editor.Layouts {
    using System.Drawing;
    using Groups;
    using UnityEngine;

    public class LayoutElement {
        public FriggProperty      property;
        public BaseGroupAttribute attribute;

        public float xOffset;
        public float yOffset;

        public LayoutElement(FriggProperty property, BaseGroupAttribute attribute) {
            this.property  = property;
            this.attribute = attribute;
        }
    }
}