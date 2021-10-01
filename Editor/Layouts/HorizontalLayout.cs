namespace Frigg.Editor.Layouts {
    using UnityEditor;
    using UnityEngine;

    public class HorizontalLayout : Layout {
        private const float HORIZONTAL_SPACING = 5.0f;
        public override void Add(FriggProperty property) {
            base.Add(property);

            var element     = this.layoutElements[this.layoutElements.Count - 1];

            element.xOffset = element.attribute.ElementWidth
                              == 0 ? 0 : element.attribute.ElementWidth;

            element.xOffset += HORIZONTAL_SPACING;

            this.layoutElements[this.layoutElements.Count - 1] = element;
        }

        protected override void Recalculate() {
            var total          = this.layoutElements.Count;
            var inspectorWidth = EditorGUIUtility.currentViewWidth;
            
            for (var i = 0; i < total; i++) {
                var element = this.layoutElements[i];
                if(element.attribute.ElementWidth == 0)
                   element.xOffset = (inspectorWidth / total);
            }
        }
    }
}