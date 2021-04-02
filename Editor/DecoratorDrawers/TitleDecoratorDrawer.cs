namespace Assets.Scripts.Editor.DecoratorDrawers {
    using System;
    using Packages.Frigg;
    using Packages.Frigg.Attributes;
    using UnityEditor;
    using UnityEngine;
    
    public class TitleDecoratorDrawer : BaseDecoratorDrawer {
        protected override float GetHeight(Rect rect) {
            var attr = (TitleAttribute) this.attribute;
            return attr.lineHeight + SPACE_AMOUNT;
        }

        protected override void DrawDecorator(Rect rect, object target) {
            var attr = (TitleAttribute) this.attribute;

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

            EditorGUI.LabelField(rect, new GUIContent(attr.Text), style);
            
            rect.y += EditorGUIUtility.singleLineHeight + attr.lineHeight * (SPACE_AMOUNT / 2f);

            rect.height = attr.lineHeight;
            
            if(attr.drawLine)
                EditorGUI.DrawRect(rect, attr.lineColor.ToColor());
        }
    }
}