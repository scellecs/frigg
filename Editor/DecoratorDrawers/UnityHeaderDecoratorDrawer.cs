namespace Frigg.Editor {
    using UnityEditor;
    using UnityEngine;
    using Utils;

    public class UnityHeaderDecoratorDrawer : FriggDecoratorDrawer {
        public override void DrawLayout() {
            EditorGUILayout.Space(5);
            var attr  = (HeaderAttribute) this.linkedAttribute;
            var style = new GUIStyle {
                fontStyle = FontStyle.Bold, fontSize = 14, 
                normal    = {textColor = new Color(225, 225, 225)}
            };
            EditorGUILayout.LabelField(attr.header, style);
            this.property.CallNextDrawer();
        }

        public override void Draw(Rect rect) {
            var       attr   = (HeaderAttribute) this.linkedAttribute;
            var       temp   = rect;
            const float height = BaseDecoratorAttribute.DEFAULT_HEIGHT + GuiUtilities.SPACE;
            temp.height = height;
            
            var style = new GUIStyle {
                fontStyle = FontStyle.Bold, fontSize = 14, 
                normal    = {textColor = new Color(225, 225, 225)}
            };
            
            EditorGUI.LabelField(temp, attr.header, style);
            temp.y += height;
            this.property.CallNextDrawer(temp);
        }

        public override bool  IsVisible   => true;
        public override float GetHeight() => GuiUtilities.SPACE + EditorGUIUtility.singleLineHeight;

    }
}