namespace Assets.Scripts.Editor.DecoratorDrawers {
    using System;
    using Packages.Frigg;
    using Packages.Frigg.Attributes;
    using UnityEditor;
    using UnityEngine;
    
    public class TitleDecoratorDrawer : BaseDecoratorDrawer {
        private const float SPACE_AMOUNT = 6.0f;

        protected override float GetHeight() {
            var attr = (TitleAttribute) this.attribute;
            return EditorGUIUtility.singleLineHeight + SPACE_AMOUNT + attr.lineHeight;
        }

        protected override void DrawDecorator(Rect rect, IDecoratorAttribute attribute) {
            var attr = (TitleAttribute) attribute;

            var style = new GUIStyle {fontSize = attr.fontSize};
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

            rect.y += SPACE_AMOUNT / 2f;

            EditorGUI.LabelField(rect, new GUIContent(attr.title), style);
            
            rect.y += EditorGUIUtility.singleLineHeight + attr.lineHeight * (SPACE_AMOUNT / 2f);

            rect.height = attr.lineHeight;
            
            if(attr.drawLine)
                EditorGUI.DrawRect(rect, attr.lineColor.ToColor());
        }
    }
}