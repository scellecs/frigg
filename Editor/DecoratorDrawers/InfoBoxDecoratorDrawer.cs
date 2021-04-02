namespace Assets.Scripts.Editor.DecoratorDrawers {
    using System;
    using Packages.Frigg.Attributes;
    using UnityEditor;
    using UnityEditor.VersionControl;
    using UnityEngine;

    public class InfoBoxDecoratorDrawer : BaseDecoratorDrawer {
        protected override float GetHeight(Rect rect) {
            var attr = (InfoBoxAttribute) this.attribute;
            
            return attr.Height;
        }

        protected override void DrawDecorator(Rect rect, object target) {
            var attr = (InfoBoxAttribute) this.attribute;

            rect.height += attr.Height;
            
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
                    var diff = Math.Abs(height - rect.height);
                    rect.height += diff;
                }
            }

            GUI.Label(rect, content, style);
            var newRect = this.GetHeight(EditorGUILayout.GetControlRect());

            if (newRect < InfoBoxAttribute.DEFAULT_HEIGHT) {
                EditorGUILayout.Space(newRect - (InfoBoxAttribute.DEFAULT_HEIGHT - newRect));
                return;
            }
            
            if (newRect > InfoBoxAttribute.DEFAULT_HEIGHT) {
                EditorGUILayout.Space(InfoBoxAttribute.DEFAULT_HEIGHT - (InfoBoxAttribute.DEFAULT_HEIGHT - newRect));
                return;
            }
            EditorGUILayout.Space(attr.Height);
        }
    }
}