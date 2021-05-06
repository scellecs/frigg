namespace Frigg.Editor {
    using Frigg;
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public class RequiredDecoratorDrawer : BaseDecoratorDrawer {
        public override bool IsVisible(SerializedProperty prop) {
           //If objectReferenceValue is null - we need to draw Required InfoBox, otherwise - return.
            if (prop.propertyType == SerializedPropertyType.ObjectReference) {
                return prop.objectReferenceValue == null;
            }

            return false;
        }

        protected override float GetHeight(Rect rect) {
            return EditorGUIUtility.singleLineHeight;
        }

        protected override void DrawDecorator(Rect rect, object target, bool isArray) {
            var attr = (RequiredAttribute) this.attribute;

            if (target.GetType() != typeof(SerializedProperty)) {
                return;
            }
            
            var serializedProperty = (SerializedProperty) target;
            if (this.property.propertyType == SerializedPropertyType.ObjectReference) {
                if (this.property.objectReferenceValue == null) {
                    if (string.IsNullOrEmpty(attr.Text)) {
                        attr.Text = $"{serializedProperty.displayName} is required!";
                    }
                }
            }
            
            else { 
                if (string.IsNullOrEmpty(attr.Text)) {
                    attr.Text = $"{serializedProperty.displayName} is required!";
                }
            }
            
            var content = EditorGUIUtility.TrTextContentWithIcon(attr.Text, MessageType.Error);
            var style = new GUIStyle(EditorStyles.helpBox) {
                fontSize  = 14,
                alignment = TextAnchor.MiddleLeft
            };

            if (!attr.HasCustomHeight) {
                var height = style.CalcHeight(content, rect.width);
                if (rect.height < height) {
                    rect.height += height - rect.height;
                }
            }

            GUI.Label(rect, content, style);

            if(!isArray) 
                EditorGUILayout.Space(this.GetHeight(EditorGUILayout.GetControlRect()));
        }
    }
}