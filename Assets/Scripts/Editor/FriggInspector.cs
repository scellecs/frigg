namespace Assets.Scripts.Editor {
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Attributes;
    using CustomPropertyDrawers;
    using PropertyDrawers;
    using UnityEditor;
    using UnityEngine;
    using Utils;
    
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true)]
    public class FriggInspector : Editor {

        private List<SerializedProperty>  serializedProperties = new List<SerializedProperty>(); //Unity's Serialized properties
        private IEnumerable<PropertyInfo> properties; //store all properties with attributes (native)
        private IEnumerable<FieldInfo>    fields;     //Store all fields with attributes (Non-serialized)
        private IEnumerable<MethodInfo>   methods;    //Store all methods with attributes (for buttons)

        private void OnEnable() {
            this.methods = this.target.TryGetMethods(info
                => info.GetCustomAttributes(typeof(ButtonAttribute), true).Length > 0);
            
            this.fields = this.target.TryGetFields(info
                => info.GetCustomAttributes(typeof(ShowInInspectorAttribute), true).Length > 0);

            this.properties = this.target.TryGetProperties(info
                => info.GetCustomAttributes(typeof(ShowInInspectorAttribute), true).Length > 0);
            
            this.serializedProperties = this.GetSerializedProperties();
            
            ReorderableListDrawer.ClearData();
        }
        
        public override void OnInspectorGUI() {
            this.serializedObject.Update();

            if(this.serializedProperties.Any(p => 
                CoreUtilities.TryGetAttribute<IAttribute>(p) != null))
                this.DrawSerializedProperties();
            else {
                this.DrawDefaultInspector();
            }
            
            this.DrawButtons();
            this.DrawNonSerializedFieldsAndProperties();
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
            foreach (var prop in this.serializedProperties
                .Where(prop => prop.name != "m_Script")) {
                prop.serializedObject.Update();
                
                GuiUtilities.PropertyField(prop, true);
            }
        }

        private void DrawNonSerializedFieldsAndProperties() {
            foreach (var field in this.fields) {
                if(field.IsUnitySerialized())
                    continue;
                
                GuiUtilities.Field(field.GetValue(this.target), field.Name);
            }

            foreach (var prop in this.properties) {
                GuiUtilities.Field(prop.GetValue(this.target), prop.Name, prop.CanWrite);
            }
        }
    }
}