namespace Assets.Scripts.Editor {
    using System.Collections.Generic;
    using System.Reflection;
    using Attributes;
    using UnityEditor;
    using UnityEngine;
    using Utils;
    
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true)]
    public class FriggInspector : Editor {

        private IEnumerable<SerializedProperty> serializedProperties;
        private IEnumerable<PropertyInfo>       properties; //store all properties with attribures
        private IEnumerable<FieldInfo>          fields;     //Store all fields with attributes
        private IEnumerable<MethodInfo>         methods;    //Store all methods with attributes (for buttons)

        private void OnEnable() {
            this.methods = this.target.TryGetMethods(info
                => info.GetCustomAttributes(typeof(ButtonAttribute), true).Length > 0);
            
            this.fields = this.target.TryGetFields(info
                => info.GetCustomAttributes(typeof(DropdownAttribute), true).Length > 0);
            //Todo: not only 'dropdown', but all other attributes.
        }
        
        public override void OnInspectorGUI() {

            this.DrawButtons();
            
            this.DrawDropdowns();
        }

        private void DrawButtons() {
            foreach (var element in this.methods) {
                GuiUtilities.Button(this.serializedObject.targetObject, element);
            }
        }

        private void DrawDropdowns() {
            foreach (var element in this.fields) {
                
            }
        }
    }
}