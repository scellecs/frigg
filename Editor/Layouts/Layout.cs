namespace Frigg.Editor.Layouts {
    using System.Collections.Generic;
    using Groups;
    using UnityEditor;
    using UnityEngine;

    public abstract class Layout {
        protected const float HORIZONTAL_SPACING = 5.0f;
        
        public bool IsListMember { get; set; }

        /// <summary>
        /// Get total height of target layout.
        /// </summary>
        public float TotalHeight => this.GetTotalHeight();

        /// <summary>
        /// Path of this layout group.
        /// </summary>
        public string layoutPath;
        
        /// <summary>
        /// Group name, depends on Attribute's 'Name" value.
        /// </summary>
        public string layoutName;

        /// <summary>
        /// List of layout elements of this Layout Group.
        /// </summary>
        protected readonly List<LayoutElement> layoutElements = new List<LayoutElement>();

        /// <summary>
        /// Add a property to target layout.
        /// </summary>
        /// <param name="property">Target property to add.</param>
        public virtual void Add(FriggProperty property) {
            var attribute = property.TryGetFixedAttribute<BaseGroupAttribute>();
            this.layoutName = attribute.GroupName;

            this.layoutPath = property.ParentProperty.Path;

            this.layoutElements.Add(new LayoutElement(property, attribute));
        }

        protected abstract void Recalculate();

        private float GetTotalHeight() {
            var height = 0f;
            foreach (var element in this.layoutElements) {
                var last = FriggProperty.GetPropertyHeight(element.property);
                if (last > height) {
                    height = last;
                }
            }
            return height;
        }

        /// <summary>
        /// Draws whole group in the provided position.
        /// </summary>
        /// <param name="rect">Start position for this group.</param>
        public void Draw(Rect rect = default) {
            this.Recalculate();
            
            foreach (var element in this.layoutElements) {
                rect.width =  element.width;
                element.property.Draw(rect);
                rect.y += element.yOffset;
                rect.x += element.width + HORIZONTAL_SPACING;
            }
        }
    }
}