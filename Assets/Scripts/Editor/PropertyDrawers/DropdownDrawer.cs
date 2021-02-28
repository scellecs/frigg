namespace Assets.Scripts.Editor.PropertyDrawers {
    using Attributes;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(DropdownAttribute))]
    public class DropdownDrawer : BaseDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndProperty();
            
            //Here we have to add an implementation of DropdownDrawer using reflection to get linked data (e.g property.name)
            //Then, we could use a custom method to draw a Dropdown. (Utils)
        }
    }
}