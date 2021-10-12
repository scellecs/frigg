namespace Frigg.Editor.Layouts {
    using UnityEditor;
    using UnityEngine;

    public class HorizontalLayout : Layout {
        public override void Add(FriggProperty property) {
            base.Add(property);

            var element   = this.layoutElements[this.layoutElements.Count - 1];
            element.width = element.attribute.ElementWidth;

            this.layoutElements[this.layoutElements.Count - 1] = element;
        }

        protected override void Recalculate() {
            var total          = this.layoutElements.Count;
            var fixedCount     = 0;
            var fixedWidth     = 0f;
            var inspectorWidth = EditorGUIUtility.currentViewWidth;

            if (this.IsListMember)
                inspectorWidth -= ReorderableListDrawer.LIST_INTERFACE_WIDTH;

            for (var i = 0; i < total; i++) {
                var element = this.layoutElements[i];
                if (element.attribute.ElementWidth == 0) {
                    continue;
                }

                fixedWidth += element.attribute.ElementWidth;
                fixedCount++;
            }

            inspectorWidth -= fixedWidth;
            var nonFixed = total - fixedCount;

            for (var i = 0; i < total; i++) {
                var element = this.layoutElements[i];
                if (element.attribute.ElementWidth == 0) {
                    element.width = inspectorWidth / nonFixed - HORIZONTAL_SPACING * fixedCount;
                }
            }
        }
    }
}