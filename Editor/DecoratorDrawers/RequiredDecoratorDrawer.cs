namespace Frigg.Editor {
    using Frigg;
    using UnityEditor;
    using UnityEngine;

    public class RequiredDecoratorDrawer : BaseDecoratorDrawer {
        protected override float GetHeight(Rect rect) {
            return EditorGUIUtility.singleLineHeight + SPACE_AMOUNT;
        }

        protected override void DrawDecorator(Rect rect, object target) {
            var attr = (RequiredAttribute) this.attribute;

            var serializedProperty = (SerializedProperty) target;

            if (this.property.propertyType == SerializedPropertyType.ObjectReference) {
                if (this.property.objectReferenceValue == null) {
                    if (string.IsNullOrEmpty(attr.Text)) {
                        attr.Text = $"{serializedProperty.displayName} is required!";
                    }
                }
            }
            
            else { 
                attr.Text = $"{nameof(RequiredAttribute)} can be used only on reference types";
            }
            
            var content = EditorGUIUtility.TrTextContentWithIcon(attr.Text, MessageType.Error);
            var style = new GUIStyle(EditorStyles.helpBox) {
                fontSize  = 14,
                alignment = TextAnchor.MiddleLeft
            };
            
            var height = style.CalcHeight(content, rect.width);
            if (rect.height < height) {
                rect.height += height - rect.height;
            }
            
            GUI.Label(rect, content, style);
            
            EditorGUILayout.Space(this.GetHeight(EditorGUILayout.GetControlRect()));
        }
    }
}