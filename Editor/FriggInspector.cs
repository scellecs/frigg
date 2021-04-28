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

        //Unity's Serialized properties (including fields)
        private List<SerializedProperty>   serializedProperties = new List<SerializedProperty>();
        private List<(MemberInfo, object)> members              = new List<(MemberInfo, object)>();

        private bool anySerialized;
        private bool hasArrays;

        private ILookup<int, object> mixedData;
        private void OnEnable() {
            this.target.TryGetMembers(this.members);

            this.serializedProperties = this.GetSerializedProperties();
            
            this.anySerialized = this.serializedProperties.Count > 0;
            
            this.hasArrays = this.serializedProperties.Any(p => p.isArray);
            
            this.mixedData = SortAll(this.serializedProperties, this.members);

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

            var keys = this.mixedData.Select(x => x.Key);

            foreach (var key in keys) {
                var elements   = this.mixedData[key];
                var enumerable = elements.ToList();

                if(!enumerable.Any())
                    continue;

                foreach (var element in enumerable) {
                    var type = element.GetType();
                    
                    if (type.FullName == null) {
                        continue;
                    }

                    if (type == typeof(ValueTuple<MemberInfo, object>)) {
                        var (memberInfo, obj) = ((MemberInfo, object)) element;
                        
                        var memberType = memberInfo.GetType();
                        
                        if (memberType.IsSubclassOf(typeof(PropertyInfo))) {
                            var propertyTuple = new ValueTuple<PropertyInfo, object>(memberInfo as PropertyInfo, obj);
                            this.DrawNativeProperty(propertyTuple);
                        }

                        if (memberType.IsSubclassOf(typeof(MethodInfo))) {
                            var methodTuple = new ValueTuple<MethodInfo, object>(memberInfo as MethodInfo, obj);
                            this.DrawButton(methodTuple);
                        }

                        if (memberType.IsSubclassOf(typeof(FieldInfo))) {
                            var fieldTuple = new ValueTuple<FieldInfo, object>(memberInfo as FieldInfo, obj);
                            this.DrawNonSerializedField(fieldTuple);
                        }
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

        private void DrawSerializedProperty(SerializedProperty prop) {
            this.serializedObject.Update();
            
            GuiUtilities.HandleDecorators(prop);
            GuiUtilities.PropertyField(prop, true);
        }
        
        private void DrawButton((MethodInfo, object) method) {
            var (methodInfo, obj) = method;
            
            GuiUtilities.HandleDecorators(methodInfo);
            GuiUtilities.Button(this.serializedObject.targetObject, methodInfo);
        }

        private void DrawNonSerializedField((FieldInfo, object) field) {
            var (fieldInfo, obj) = field;
            
            if (fieldInfo.IsUnitySerialized()) {
                return;
            }
            
            var value = fieldInfo.GetValue(obj);

            GuiUtilities.HandleDecorators(obj.GetType());
            GuiUtilities.HandleDecorators(fieldInfo);
            var content  = CoreUtilities.GetGUIContent(fieldInfo);
            var writable = CoreUtilities.IsWritable(fieldInfo);

            if (typeof(IEnumerable).IsAssignableFrom(fieldInfo.FieldType) && fieldInfo.FieldType != typeof(string)) {
                var drawer = (ReorderableListDrawer) CustomAttributeExtensions.GetCustomDrawer(typeof(ReorderableListAttribute));
                drawer.OnGUI(obj, Rect.zero, fieldInfo, content);
                return;
            }
            
            fieldInfo.SetValue(obj, GuiUtilities.LayoutField(fieldInfo.FieldType, value, content, writable));
        }
        
        private void DrawNativeProperty((PropertyInfo, object) property) {
            var (propertyInfo, obj) = property;
            
            GuiUtilities.HandleDecorators(obj.GetType());
            GuiUtilities.HandleDecorators(propertyInfo);
            
            var content = CoreUtilities.GetGUIContent(propertyInfo);
            
            if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType) && propertyInfo.PropertyType != typeof(string)) {
                var drawer = (ReorderableListDrawer) CustomAttributeExtensions.GetCustomDrawer(typeof(ReorderableListAttribute));
                drawer.OnGUI(obj, Rect.zero, propertyInfo, content);
                return;
            }

            var writable = CoreUtilities.IsWritable(propertyInfo);
            var value    = propertyInfo.GetValue(obj);

            if (CoreUtilities.IsPrimitiveUnityType(propertyInfo.PropertyType)) {
                if(writable)
                   propertyInfo.SetValue(obj, GuiUtilities
                       .LayoutField(propertyInfo.PropertyType, propertyInfo.GetValue(obj), content));
                else { 
                    GuiUtilities.LayoutField(propertyInfo.PropertyType, propertyInfo.GetValue(obj), content, false);
                }
            }

            else {
                propertyInfo.SetValue(obj, GuiUtilities
                    .MultiFieldLayout(propertyInfo, propertyInfo.GetValue(obj), content, writable));
            }

            var secondValue = propertyInfo.GetValue(obj);

            if (value.Equals(secondValue) || Application.isPlaying) {
                return;
            }
                
            EditorUtility.SetDirty(this.target);
        }

        private static ILookup<int, object> SortAll(IEnumerable<SerializedProperty> serProps,
            IEnumerable<(MemberInfo, object)> members) {
            var pairs = new List<KeyValuePair<int, object>>();

            foreach (var prop in serProps) {
                var attr = CoreUtilities.TryGetAttribute<OrderAttribute>(prop);
                if (prop.name == "m_Script") {
                    pairs.Add(new KeyValuePair<int, object>(-1000, prop));
                    continue;
                }

                pairs.Add(attr != null
                    ? new KeyValuePair<int, object>(attr.Order, prop)
                    : new KeyValuePair<int, object>(0, prop));
            }

            foreach (var member in members) {
                var attr  = (OrderAttribute) member.Item1.GetCustomAttributes(typeof(OrderAttribute)).FirstOrDefault();
                pairs.Add(attr != null
                    ? new KeyValuePair<int, object>(attr.Order, member)
                    : new KeyValuePair<int, object>(0, member));
            }

            var kvPairs = pairs.OrderBy(s => s.Key);
            return kvPairs.ToLookup(pair =>
                pair.Key, pair => pair.Value);
        }
    }
} 