namespace Assets.Scripts.Editor {
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Attributes;
    using Attributes.Custom;
    using Attributes.Meta;
    using CustomPropertyDrawers;
    using UnityEditor;
    using UnityEngine;
    using Utils;
    
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true)]
    public class FriggInspector : Editor {

        private List<SerializedProperty>  serializedProperties = new List<SerializedProperty>(); //Unity's Serialized properties (including fields)
        private IEnumerable<PropertyInfo> properties; //store all properties with attributes (native)
        private IEnumerable<FieldInfo>    fields;     //Store all fields with attributes (Non-serialized)
        private IEnumerable<MethodInfo>   methods;    //Store all methods with attributes (for buttons)

        private ILookup<int, object> mixedData;
        private void OnEnable() {
            this.methods = this.target.TryGetMethods(info
                => info.GetCustomAttributes(typeof(ButtonAttribute), true).Length > 0);
            
            this.fields = this.target.TryGetFields(info
                => info.GetCustomAttributes(typeof(ShowInInspectorAttribute), true).Length > 0);

            this.properties = this.target.TryGetProperties(info
                => info.GetCustomAttributes(typeof(ShowInInspectorAttribute), true).Length > 0);
            
            this.serializedProperties = this.GetSerializedProperties();
            
            this.mixedData = SortAll(this.serializedProperties, 
                this.properties, this.fields, this.methods);

            ReorderableListDrawer.ClearData();
        }
        
        public override void OnInspectorGUI() {
            this.serializedObject.Update();

            if (this.serializedProperties.All(p => CoreUtilities.TryGetAttribute<IAttribute>(p) == null))
                this.DrawDefaultInspector();

            var max    = this.mixedData.Max(x => x.Key);

            for (var i = 0; i <= max; i++) {
                var elements = this.mixedData[i];

                foreach (var element in elements) {
                    if (element == null)
                        return;

                    var type = element.GetType();

                    if (type.FullName == null) {
                        continue;
                    }

                    //Todo: Handle this better. (e.g type == typeof(PropertyField)...)
                    if (type.FullName.Contains("MonoProperty")) {
                        this.DrawNativeProperty((PropertyInfo) element);
                    }

                    if (type.FullName.Contains("MonoMethod")) {
                        this.DrawButton((MethodInfo) element);
                    }

                    if (type.FullName.Contains("MonoField")) {
                        this.DrawNonSerializedField((FieldInfo) element);
                    }

                    if (type != typeof(SerializedProperty)) {
                        continue;
                    }

                    var prop = (SerializedProperty) element;
                    if (prop.name == "m_Script")
                        continue;

                    this.DrawSerializedProperty((SerializedProperty) element);
                }
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

        private void DrawButton(MethodInfo element) {
            GuiUtilities.Button(this.serializedObject.targetObject, element);
        }

        private void DrawSerializedProperty(SerializedProperty prop) {
            this.serializedObject.Update();
            GuiUtilities.PropertyField(prop, true);
        }

        private void DrawNonSerializedField(FieldInfo field) {

            if (field.IsUnitySerialized()) {
                return;
            }

            var value = field.GetValue(this.target);
            if (value == null)
                return;
            field.SetValue(this.target,
                GuiUtilities.Field(field.GetValue(this.target), $"[private] {field.Name}"));
        }

        private void DrawNativeProperty(PropertyInfo prop) {
            if (!prop.CanWrite) {
                GuiUtilities.Field(prop.GetValue(this.target), $"[property] {prop.Name}", false);
            }

            else {
                var value = prop.GetValue(this.target);
                    
                prop.SetValue(this.target, GuiUtilities
                    .Field(prop.GetValue(this.target), $"[property] {prop.Name}"));

                var secondValue = prop.GetValue(this.target);

                if (value.Equals(secondValue) || Application.isPlaying) {
                    return;
                }
                
                EditorUtility.SetDirty(this.target);
            }
        }

        private static ILookup<int, object> SortAll(IEnumerable<SerializedProperty> serProps, 
            IEnumerable<PropertyInfo> props, IEnumerable<FieldInfo> fields, 
            IEnumerable<MethodInfo> methods) {

            var pairs = new List<KeyValuePair<int, object>>();
            
            foreach (var field in fields) {
                var attr = (OrderAttribute) field.GetCustomAttributes(typeof(OrderAttribute)).First();
                if (attr != null) {
                    pairs.Add(new KeyValuePair<int, object>(attr.Order, field));
                }
            }
            
            foreach (var prop in props) {
                var attr = (OrderAttribute) prop.GetCustomAttributes(typeof(OrderAttribute)).First();
                if (attr != null) {
                    pairs.Add(new KeyValuePair<int, object>(attr.Order, prop));
                }
            }
            
            foreach (var method in methods) {
                var attr = (OrderAttribute) method.GetCustomAttributes(typeof(OrderAttribute)).First();
                if (attr != null) {
                    pairs.Add(new KeyValuePair<int, object>(attr.Order, method));
                }
            }

            foreach (var prop in serProps) {
                var attr = CoreUtilities.TryGetAttribute<OrderAttribute>(prop);
                if (attr != null) {
                    pairs.Add(new KeyValuePair<int, object>(attr.Order, prop));
                }
            }

            return pairs.OrderBy(s => s.Key).ToLookup(pair =>
                pair.Key, pair => pair.Value);
        }
    }
} 