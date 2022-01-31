namespace Frigg.Editor {
    using System;
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public class InfoBoxDecoratorDrawer : FriggDecoratorDrawer {
        public override void DrawLayout() {
            var attr = (InfoBoxAttribute) this.linkedAttribute;

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

            //todo: move this code into another class. This should be checked only before calling "Property.Draw"
            if (!string.IsNullOrEmpty(attr.Member)) {
                    //todo: get "member" name & call directly.
            }
            else {
                EditorGUILayout.HelpBox(attr.Text, messageType);
            }

            this.property.CallNextDrawer();
        }

        public override void Draw(Rect rect) {
            var       temp   = rect;
            const int height = BaseDecoratorAttribute.DEFAULT_HEIGHT;
            temp.height = height;
            var infoBox = (InfoBoxAttribute) this.linkedAttribute;
            
            EditorGUI.HelpBox(temp, infoBox.Text, this.GetMessageType(infoBox.InfoMessageType));
            rect.y += height;
            this.property.CallNextDrawer(rect);
        }

        private MessageType GetMessageType(InfoMessageType type) {
            switch (type) {
                case InfoMessageType.None:
                    return MessageType.None;
                
                case InfoMessageType.Info:
                    return MessageType.Info;
                
                case InfoMessageType.Warning:
                    return MessageType.Warning;
                
                case InfoMessageType.Error:
                    return MessageType.Error;
                
                default:
                    return MessageType.Info;
            }
        }

        public override bool  IsVisible   => true;
        public override float GetHeight() => ((InfoBoxAttribute)this.linkedAttribute).Height;
    }   
}