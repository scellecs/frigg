namespace Assets.Scripts.Editor.DecoratorDrawers {
    using System;
    using Packages.Frigg;
    using Packages.Frigg.Attributes;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(TitleAttribute))]
    public class TitleDecoratorDrawer : DecoratorDrawer {
        private const float SPACE_AMOUNT = 8.0f;
        
        public override float GetHeight() {
            var attr = (TitleAttribute) attribute;

            //Spacing between each element
            return EditorGUIUtility.singleLineHeight + SPACE_AMOUNT + attr.lineHeight;
        }

        public override void OnGUI(Rect position) {
            var attr = (TitleAttribute) this.attribute;

            var rect = EditorGUI.IndentedRect(position);

            var style = new GUIStyle {fontSize = attr.fontSize};
            if (attr.bold) {
                style.fontStyle     = FontStyle.Bold;
            }
            
            style.normal.textColor  = attr.textColor.ToColor();

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

            EditorGUI.LabelField(rect, new GUIContent(attr.title), style);
            
            rect.y += EditorGUIUtility.singleLineHeight + attr.lineHeight * (SPACE_AMOUNT / 2f);

            rect.height =  attr.lineHeight;

            if(attr.drawLine)
               EditorGUI.DrawRect(rect, attr.lineColor.ToColor());
        }
    }
}