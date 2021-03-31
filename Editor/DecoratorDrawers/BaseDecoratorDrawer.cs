namespace Assets.Scripts.Editor.DecoratorDrawers {
    using Packages.Frigg.Attributes;
    using UnityEditor;
    using UnityEngine;

    public abstract class BaseDecoratorDrawer {
        public BaseDecoratorAttribute attribute;
        
        public void OnGUI(Rect rect, IDecoratorAttribute attr) {

            this.attribute = (BaseDecoratorAttribute)attr;
            
            var height = this.GetHeight();
            EditorGUILayout.Space(height / 2f);

            this.DrawDecorator(rect, attr);
        }

        protected abstract float GetHeight();

        protected abstract void DrawDecorator(Rect rect, IDecoratorAttribute attribute);
    }
}