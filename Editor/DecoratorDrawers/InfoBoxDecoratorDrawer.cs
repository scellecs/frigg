namespace Frigg.Editor {
    using System;
    using UnityEditor;
    using UnityEngine;

    public class InfoBoxDecoratorDrawer : BaseDecoratorDrawer {
        protected override float GetHeight(Rect rect) {
            var attr = (InfoBoxAttribute) this.attribute;
            
            return attr.Height - EditorGUIUtility.singleLineHeight;
        }
        
        protected override void DrawDecorator(Rect rect, object target, bool isArray) {
            var attr = (InfoBoxAttribute) this.attribute;

            MessageType messageType;
            switch (attr.InfoMessageType) {
                case InfoMessageType.None:
                    messageType = MessageType.None;
                    break;
                case InfoMessageType.Info:
                    messageType = MessageType.Info;
                    break;
                case InfoMessageType.Warning:
                    messageType = MessageType.Warning;
                    break;
                case InfoMessageType.Error:
                    messageType = MessageType.Error;
                    break;
                default:
                    messageType = MessageType.Info;
                    break;
            }

            var content = EditorGUIUtility.TrTextContentWithIcon(attr.Text, messageType);
            var style   = new GUIStyle(EditorStyles.helpBox) {
                fontSize = attr.FontSize,
                alignment = TextAnchor.MiddleLeft
            };
            
            //we need to recalculate height each time our text wrapping to the next line
            if (!attr.HasCustomHeight) {
                var height = style.CalcHeight(content, rect.width);
                if (rect.height < height) { 
                    var diff    =  Math.Abs(height - rect.height);
                    rect.height += diff;
                }
            }

            GUI.Label(rect, content, style);
            
            if(!isArray)
                EditorGUILayout.Space(this.GetHeight(EditorGUILayout.GetControlRect()));
        }
    }   
}