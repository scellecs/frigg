namespace Frigg.Editor {
    using System;
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

            if (CoreUtilities.IsPropertyVisible(this.property)) {
                if (!string.IsNullOrEmpty(attr.Member)) {
                    var value = (bool) CoreUtilities.GetTargetObject(this.property.ParentProperty.PropertyValue.Value,
                        this.property.ParentProperty.PropertyValue.Value.GetType().GetMember(attr.Member, CoreUtilities.FLAGS)[0]);
                    if (value) {
                        EditorGUILayout.HelpBox(attr.Text, messageType);
                    }
                }
                else {
                    EditorGUILayout.HelpBox(attr.Text, messageType);
                }
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
        /*public override bool IsVisible(SerializedProperty prop) => true;

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
                rect.height = BaseDecoratorAttribute.DEFAULT_HEIGHT;
                var height  = style.CalcHeight(content, EditorGUIUtility.currentViewWidth);
                if (rect.height < height) { 
                    var diff    =  Math.Abs(height - rect.height);
                    rect.height += diff;
                }
            }

            else {
                rect.height = this.attribute.Height;
            }

            GUI.Label(rect, content, style);
            
            if(!isArray)
                EditorGUILayout.Space(this.GetHeight(EditorGUILayout.GetControlRect()));
        }*/
    }   
}