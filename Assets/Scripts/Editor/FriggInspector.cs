namespace Assets.Scripts.Editor {
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Attributes;
    using UnityEditor;
    using UnityEngine;
    using Utils;
    
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true)]
    public class FriggInspector : Editor {

        private List<SerializedProperty>  serializedProperties = new List<SerializedProperty>();
        private IEnumerable<PropertyInfo> properties; //store all properties with attributes
        private IEnumerable<FieldInfo>    fields;     //Store all fields with attributes
        private IEnumerable<MethodInfo>   methods;    //Store all methods with attributes (for buttons)

        private void OnEnable() {
            this.methods = this.target.TryGetMethods(info
                => info.GetCustomAttributes(typeof(ButtonAttribute), true).Length > 0);
            
            this.fields = this.target.TryGetFields(info
                => info.GetCustomAttributes(typeof(DropdownAttribute), true).Length > 0);
            //Todo: not only 'dropdown', but all other attributes.

            this.serializedProperties = this.GetSerializedProperties();
        }
        
        public override void OnInspectorGUI() {

            this.DrawButtons();

            if(this.serializedProperties.Any(p => 
                CoreUtilities.TryGetAttribute<IAttribute>(p) != null))
                this.DrawSerializedProperties();
            else {
                this.DrawDefaultInspector();
            }
        }

        private List<SerializedProperty> GetSerializedProperties() {
            var list = new List<SerializedProperty>();

            var it = this.serializedObject.GetIterator();

            if (!it.NextVisible(true)) {
                return list;
            }

            do {
                var prop = this.serializedObject.FindProperty(it.name);
                list.Add(prop);
            }
            while (it.NextVisible(false));

            return list;
        }

        private void DrawButtons() {
            foreach (var element in this.methods) {
                GuiUtilities.Button(this.serializedObject.targetObject, element);
            }
        }

        private void DrawSerializedProperties() {
            foreach (var prop in this.serializedProperties) {
                if (prop.name == "m_Script") {
                    continue;
                }
                GuiUtilities.PropertyField(prop, true);
            }
        }
    }
}