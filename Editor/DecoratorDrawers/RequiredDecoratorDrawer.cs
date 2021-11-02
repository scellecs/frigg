namespace Frigg.Editor {
    using Frigg;
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public class RequiredDecoratorDrawer : FriggDecoratorDrawer {
        public override void DrawLayout() {
            EditorGUILayout.HelpBox($"{this.property.NiceName} is required!", MessageType.Error);
            this.property.CallNextDrawer();
        }

        public override void Draw(Rect rect) {
            var       temp   = rect;
            const int height = BaseDecoratorAttribute.DEFAULT_HEIGHT;
            temp.height =  height;
            temp.width  -= EditorGUI.indentLevel * 15; //to const
            temp.x      += EditorGUI.indentLevel * 15;
            EditorGUI.HelpBox(temp, $"{this.property.NiceName} is required!", MessageType.Error);
            rect.y += height;
            this.property.CallNextDrawer(rect);
        }

        public override bool IsVisible {
            get {
                var value = this.property.GetValue();
                if (value == default) {
                    return true;
                }

                return false;
            }
        }
        
        public override float GetHeight() => BaseDecoratorAttribute.DEFAULT_HEIGHT;
    }
}