namespace Assets.Scripts.Editor.DecoratorDrawers {
    using Packages.Frigg.Attributes;
    using UnityEditor;
    using UnityEngine;

    public abstract class BaseDecoratorDrawer {
        public       BaseDecoratorAttribute attribute;
        public const float                  SPACE_AMOUNT = 8.0f;
        
        public void OnGUI(Rect rect, IDecoratorAttribute attr) {

            this.attribute = (BaseDecoratorAttribute)attr;
            
            var height = this.GetHeight();
            EditorGUILayout.Space(height);

            this.DrawDecorator(rect);
        }

        protected abstract float GetHeight();

        protected abstract void DrawDecorator(Rect rect);
    }
}