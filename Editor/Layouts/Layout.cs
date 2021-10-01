namespace Frigg.Editor.Layouts {
    using System.Collections.Generic;
    using Groups;
    using UnityEditor;
    using UnityEngine;

    public abstract class Layout {
        public float totalHeight => this.GetTotalHeight();

        public string layoutPath;
        public string layoutName;

        //To dictionary or hashset
        protected List<LayoutElement> layoutElements = new List<LayoutElement>();

        public virtual void Add(FriggProperty property) {
            var attribute = property.TryGetFixedAttribute<BaseGroupAttribute>();
            this.layoutName = attribute.GroupName;

            this.layoutPath = property.ParentProperty.Path;

            this.layoutElements.Add(new LayoutElement(property, attribute));
        }

        public virtual void Remove() {
            
        }

        protected abstract void Recalculate();

            private float GetTotalHeight() {
            var height = 0;
            foreach (var element in this.layoutElements) {
                
            }

            return height;
        }

        /// <summary>
        /// Draws whole group in the provided position.
        /// </summary>
        /// <param name="rect">Start position for this group</param>
        public void Draw(Rect rect = default) {
            this.Recalculate();
            foreach (var element in this.layoutElements) {
                element.property.Draw(rect);
                rect.x      += element.xOffset;
                rect.width -= element.xOffset;
            }
        }
    }
}