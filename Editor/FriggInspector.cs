namespace Frigg.Editor {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using Utils;
    using Object = UnityEngine.Object;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true)]
    public class FriggInspector : Editor {

        private List<SerializedProperty>  serializedProperties = new List<SerializedProperty>(); //Unity's Serialized properties (including fields)
        private IEnumerable<PropertyInfo> properties; //store all properties with attributes (native)
        private IEnumerable<FieldInfo>    fields;     //Store all fields with attributes (Non-serialized)
        private IEnumerable<MethodInfo>   methods;    //Store all methods with attributes (for buttons)

        private bool anySerialized;
        private bool hasArrays;

        private ILookup<int, object> mixedData;
        private void OnEnable() {
            this.methods = this.target.TryGetMethods(info
                => info.GetCustomAttributes(typeof(ButtonAttribute), true).Length > 0);
            
            this.fields = this.target.TryGetFields(info
                => info.GetCustomAttributes(typeof(ShowInInspectorAttribute), true).Length > 0);

            this.properties = this.target.TryGetProperties(info
                => info.GetCustomAttributes(typeof(ShowInInspectorAttribute), true).Length > 0);
            
            this.serializedProperties = this.GetSerializedProperties();
            
            this.anySerialized = this.serializedProperties.Count > 0;
            
            this.hasArrays = this.serializedProperties.Any(p => p.isArray);
            
            this.mixedData = SortAll(this.serializedProperties, 
                this.properties, this.fields, this.methods);

            ReorderableListDrawer.ClearData();
        }
        
        public override void OnInspectorGUI() {
            this.serializedObject.Update();

            //means there's no arrays and props with attributes at all.
            if (!this.anySerialized && !this.hasArrays) {
                this.DrawDefaultInspector();
            }

            if(this.mixedData.Count <= 0)
                return;

            var min = this.mixedData.Min(x => x.Key);
            var max = this.mixedData.Max(x => x.Key);

            for (var i = min; i <= max; i++) {
                var elements = this.mixedData[i];

                var enumerable = elements.ToList();
                if(!enumerable.Any())
                    continue;

                foreach (var element in enumerable) {
                    var type = element.GetType().GetTypeInfo();

                    if (type.FullName == null) {
                        continue;
                    }

                    if (type.IsSubclassOf(typeof(PropertyInfo))) {
                        this.DrawNativeProperty((PropertyInfo) element);
                    }
                    
                    if (type.IsSubclassOf(typeof(MethodInfo))) {
                        this.DrawButton((MethodInfo) element);
                    }

                    if (type.IsSubclassOf(typeof(FieldInfo))) {
                        this.DrawNonSerializedField((FieldInfo) element);
                    }
                    
                    if (type != typeof(SerializedProperty)) {
                        continue;
                    }

                    var prop = (SerializedProperty) element;

                    if (prop.name == "m_Script") {
                        var propType = CoreUtilities.GetTargetObjectWithProperty(prop).GetType();
                        var hasAttr  = (HideMonoScriptAttribute) Attribute.GetCustomAttribute(propType,
                            typeof(HideMonoScriptAttribute));
                        
                        if (hasAttr != null) {
                            continue;
                        }
                    }

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
            GuiUtilities.HandleDecorators(element);
            GuiUtilities.Button(this.serializedObject.targetObject, element);
        }

        private void DrawSerializedProperty(SerializedProperty prop) {
            this.serializedObject.Update();

            GuiUtilities.HandleDecorators(prop);
            GuiUtilities.PropertyField(prop, true);
        }

        private void DrawNonSerializedField(FieldInfo field) {
            if (field.IsUnitySerialized()) {
                return;
            }

            var value = field.GetValue(this.target);
            if (value == null) {
                //we need to check this because string is a reference type so it's always null on init.
                if(!field.FieldType.IsValueType) {
                    value = string.Empty;
                }
                else
                   return;
            }

            var canWrite = field.GetCustomAttribute<ReadonlyAttribute>() == null;

            GuiUtilities.HandleDecorators(field);
            
            var content = CoreUtilities.GetGUIContent(field);

            if (typeof(IEnumerable).IsAssignableFrom(field.FieldType) && field.FieldType != typeof(string)) {
                var drawer = (ReorderableListDrawer) CustomAttributeExtensions.GetCustomDrawer(typeof(ReorderableListAttribute));
                drawer.OnGUI(this.target, Rect.zero, field, content);
                return;
            }

            field.SetValue(this.target,
                GuiUtilities.LayoutField(value, content, canWrite));
        }

        private void DrawNativeProperty(PropertyInfo prop) {

            GuiUtilities.HandleDecorators(prop);

            var content = CoreUtilities.GetGUIContent(prop);
            
            if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string)) {
                var drawer = (ReorderableListDrawer) CustomAttributeExtensions.GetCustomDrawer(typeof(ReorderableListAttribute));
                drawer.OnGUI(this.target, Rect.zero, prop, content);
                return;
            }
            
            if (!prop.CanWrite || prop.GetCustomAttribute<ReadonlyAttribute>() != null) {
                GuiUtilities.LayoutField(prop.GetValue(this.target), content, false);
            }

            else {
                var value = prop.GetValue(this.target);
                    
                prop.SetValue(this.target, GuiUtilities
                    .LayoutField(prop.GetValue(this.target), content));

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

            foreach (var method in methods) {
                var attr = (OrderAttribute) method.GetCustomAttributes(typeof(OrderAttribute)).FirstOrDefault();
                pairs.Add(attr != null
                    ? new KeyValuePair<int, object>(attr.Order, method)
                    : new KeyValuePair<int, object>(0, method));
            }

            foreach (var prop in serProps) {
                var attr = CoreUtilities.TryGetAttribute<OrderAttribute>(prop);
                pairs.Add(attr != null
                    ? new KeyValuePair<int, object>(attr.Order, prop)
                    : new KeyValuePair<int, object>(0, prop));
            }

            foreach (var field in fields) {
                var attr = (OrderAttribute) field.GetCustomAttributes(typeof(OrderAttribute)).FirstOrDefault();
                pairs.Add(attr != null
                    ? new KeyValuePair<int, object>(attr.Order, field)
                    : new KeyValuePair<int, object>(0, field));
            }

            foreach (var prop in props) {
                var attr = (OrderAttribute) prop.GetCustomAttributes(typeof(OrderAttribute)).FirstOrDefault();
                pairs.Add(attr != null
                    ? new KeyValuePair<int, object>(attr.Order, prop)
                    : new KeyValuePair<int, object>(0, prop));
            }

            var kvPairs = pairs.OrderBy(s => s.Key);
            return kvPairs.ToLookup(pair =>
                pair.Key, pair => pair.Value);
        }
    }
} 