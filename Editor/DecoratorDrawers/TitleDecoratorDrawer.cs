namespace Frigg.Editor {
    using Frigg;
    using UnityEditor;
    using UnityEngine;

    public class TitleDecoratorDrawer : FriggDecoratorDrawer {
        public override void DrawLayout() {
            var attr = (TitleAttribute) this.linkedAttribute;

            var controlRect = EditorGUILayout.GetControlRect(false);
            var rect    = new Rect(controlRect.x, controlRect.y, controlRect.width, attr.Height);
            
            var style       = new GUIStyle {fontSize = attr.fontSize};
            if (attr.bold) {
                style.fontStyle = FontStyle.Bold;
            }
            
            style.normal.textColor = attr.textColor.ToColor();

            switch (attr.titleAlighment) {
                case TitleAlignment.Left:
                    style.alignment = TextAnchor.MiddleLeft;
                    break;
                case TitleAlignment.Right:
                    style.alignment = TextAnchor.MiddleRight;
                    break;
                case TitleAlignment.Centered:
                    style.alignment = TextAnchor.MiddleCenter;
                    break;
            }

            EditorGUI.LabelField(rect, new GUIContent(attr.Text), style);
            
            rect.y += EditorGUIUtility.singleLineHeight;

            rect.height = attr.lineHeight;
            
            if(attr.drawLine)
                EditorGUI.DrawRect(rect, attr.lineColor.ToColor());
            
            this.property.CallNextDrawer();
        }

        public override void Draw(Rect rect) {
            
            //TODO: Rect implementation
            this.property.CallNextDrawer(rect);
        }

        public override bool IsVisible => true;
        
        public override float GetHeight() =>  ((TitleAttribute)this.linkedAttribute).Height = ((TitleAttribute)this.linkedAttribute).lineHeight;
    }
}