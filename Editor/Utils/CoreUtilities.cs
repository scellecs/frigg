using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Frigg.Utils {
    using Editor;
    using Object = UnityEngine.Object;

    public static class CoreUtilities {
        public const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.NonPublic
                                                                | BindingFlags.Public
                                                                | BindingFlags.DeclaredOnly;

        #region reflection
        
        public static bool IsWritable(MemberInfo member) {
            var writable = !member.IsDefined(typeof(ReadonlyAttribute));

            if (member.MemberType != MemberTypes.Property) {
                return writable;
            }

            var property = (PropertyInfo) member;
            writable = property.CanWrite;

            return writable;
        }

        public static void HandleMetaAttributes(this FriggProperty property) {
            var hideLabelAttribute = property.TryGetFixedAttribute<HideLabelAttribute>();
            if (hideLabelAttribute != null) {
                property.Label = GUIContent.none;
                return;
            }

            var labelText = property.TryGetFixedAttribute<LabelText>();
            if (labelText != null) {
                if (labelText.IsDynamic) {
                    property.Label.text = (string)GetValueByName(property.ParentValue, labelText.Text);
                }

                else {
                    property.Label.text = labelText.Text;
                }
            }
            
            var toolTip = property.TryGetFixedAttribute<PropertyTooltipAttribute>();
            if (toolTip != null) {
                if (toolTip.IsDynamic) {
                    property.Label.tooltip = (string) GetValueByName(property.ParentValue, toolTip.Text);
                }

                else {
                    property.Label.tooltip = toolTip.Text;
                }
            }
        }

        public static object GetValueByName(object target, string name) {
            var info = target.GetType().GetMembers(FLAGS)[0];
            
            if (info is PropertyInfo propertyInfo) {
                return propertyInfo.GetValue(target);
            }

            if (info is FieldInfo fieldInfo) {
                return fieldInfo.GetValue(target);
            }

            return null;
        }

        public static IEnumerable<MethodInfo> TryGetMethods(this object target, Func<MethodInfo, bool> predicate) {
            if (target != null) {
                var type = target.GetType();
                var data = new List<MethodInfo>();
                data.AddRange(target.GetType()
                    .GetMethods(BindingFlags.Instance
                                | BindingFlags.NonPublic 
                                | BindingFlags.Public).Where(predicate));

                type = type.BaseType;
                while (type != null) {
                    data.AddRange(type.GetMethods(
                        BindingFlags.Instance | 
                        BindingFlags.NonPublic |
                        BindingFlags.Public).Where(predicate));

                    type = type.BaseType;
                }

                return data;
            }

            Debug.LogError("There's no target specified.");
            return null;
        }
        
        public static IEnumerable<FieldInfo> TryGetFields(this object target, Func<FieldInfo, bool> predicate) {
            if (target != null) {
                var type = target.GetType();
                var data = new List<FieldInfo>();
                data.AddRange(target.GetType()
                    .GetFields(FLAGS).Where(predicate));

                type = type.BaseType;
                while (type != null) {
                    data.AddRange(type.GetFields(
                        FLAGS).Where(predicate));

                    type = type.BaseType;
                }
                
                return data;
            }
        
            Debug.LogError("There's no target specified.");
            return null;
        }
    
        public static IEnumerable<PropertyInfo> TryGetProperties(this object target, Func<PropertyInfo, bool> predicate) {
            if (target != null) {
                var type = target.GetType();
                var data = new List<PropertyInfo>();
                data.AddRange(target.GetType()
                    .GetProperties(FLAGS).Where(predicate));

                type = type.BaseType;
                while (type != null) {
                    data.AddRange(type.GetProperties(
                        FLAGS).Where(predicate));

                    type = type.BaseType;
                }
            
                return data;
            }
        
            Debug.LogError("There's no target specified.");
            return null;
        }

        public static void TryGetMembers(this object target, List<PropertyValue> info) {
            if (target != null) {
                //get type of provided target
                var      type = target.GetType();

                while (type != null) {
                    //Get all fields in provided type
                    var methods = type.GetMethods(FLAGS);

                    //Get all methods in provided type
                    var fields = type.GetFields(FLAGS);
                    
                    //Get all properties in provided type
                    var properties = type.GetProperties(FLAGS);

                    foreach (var method in methods) {
                        if (!method.IsDefined(typeof(ButtonAttribute))) {
                            continue;
                        }
                        
                        var orderAttr = method.GetCustomAttribute<OrderAttribute>();
                        var order     = orderAttr?.Order ?? 0;
                        
                        var member = new PropertyValue {MetaInfo = new PropertyMeta {
                            Name       =  method.Name,
                            MemberType = method.ReturnType,
                            MemberInfo = method,
                            Order = order
                        }, target = target};
                        if (!info.Contains(member)) {
                            info.Add(member);
                        }
                    }

                    for (var i = 0; i < fields.Length; ++i) {
                        if (fields[i].IsDefined(typeof(SerializableAttribute))
                            || fields[i].IsDefined(typeof(SerializeField))
                            || fields[i].IsDefined(typeof(ShowInInspectorAttribute)) 
                            || fields[i].IsPublic) {
                            var orderAttr = fields[i].GetCustomAttribute<OrderAttribute>();
                            var order     = orderAttr?.Order ?? 0;

                            var member = new PropertyValue {MetaInfo = new PropertyMeta {
                                Name       =  fields[i].Name,
                                MemberType =  fields[i].FieldType,
                                MemberInfo = fields[i],
                                Order = order
                            }, target = target};
                        
                            if (!info.Contains(member)) {
                                info.Add(member);
                            }
                        }
                    }

                    for (var i = 0; i < properties.Length; ++i) {
                        if (!properties[i].IsDefined(typeof(SerializableAttribute)) 
                            && !properties[i].IsDefined(typeof(ShowInInspectorAttribute))) {
                            continue;
                        }
                        
                        var orderAttr = properties[i].GetCustomAttribute<OrderAttribute>();
                        var order     = orderAttr?.Order ?? 0;
                        
                        var member = new PropertyValue {MetaInfo = new PropertyMeta {
                            Name       =  properties[i].Name,
                            MemberType = properties[i].PropertyType,
                            MemberInfo = properties[i],
                            Order = order
                        }, target = target};
                        
                        if (!info.Contains(member)) {
                            info.Add(member);
                        }
                    }

                    type = type.BaseType;
                }

                return;
            }
        
            Debug.LogError("There's no target specified.");
        }

        //TODO: fix allocations
        public static MethodInfo TryGetMethod(this object target, string name) {
            if(target != null)
               return  target.TryGetMethods(x => x.Name == name).FirstOrDefault();

            Debug.LogError("There's no target specified.");
            return null;
        }
    
        //TODO: fix allocations
        public static FieldInfo TryGetField(this object target, string name) {
            if (target != null) {
                return  target.TryGetFields(x => x.Name == name).FirstOrDefault();
            }

            Debug.LogError("There's no target specified.");
            return null;
        }
    
        //TODO: fix allocations
        public static PropertyInfo TryGetProperty(this object target, string name) {
            if (target != null) {
                return  target.TryGetProperties(x => x.Name == name).FirstOrDefault();
            }

            Debug.LogError("There's no target specified.");
            return null;
        }
    
        public static Type TryGetListElementType(Type listType) 
            => listType.IsGenericType ? listType.GetGenericArguments()[0] : listType.GetElementType();

        #endregion

        #region properties //TODO: Check for any kind of errors.
    
        public static Type GetPropertyType(FriggProperty property)
        {

            return property.GetType();
        }

        public static bool IsPropertyVisible(FriggProperty property) {
            ConditionAttribute attr = property.TryGetFixedAttribute<HideIfAttribute>();

            if (attr != null) {
                return GetConditionValue(property, attr, false);
            }

            attr = property.TryGetFixedAttribute<ShowIfAttribute>();
            return attr == null || GetConditionValue(property, attr, true);
        }

        public static bool IsPropertyEnabled(FriggProperty property) {
            ConditionAttribute attr = property.TryGetFixedAttribute<DisableIfAttribute>();

            if (attr != null) {
                return GetConditionValue(property, attr, false);
            }

            attr = property.TryGetFixedAttribute<EnableIfAttribute>();
            return attr == null || GetConditionValue(property, attr, true);
        }

        private static bool GetConditionValue(FriggProperty property, ConditionAttribute attr, bool invertedScope) {
            var target = property.ParentValue;
            var field  = target.TryGetField(attr.FieldName);
            if (field == null) {
                return true;
            }
            
            var fieldValue = field.GetValue(target);

            if (fieldValue is bool boolValue) {
                if (!invertedScope) {
                    return boolValue != attr.Condition;
                }

                return boolValue == attr.Condition;
            }

            var fieldValueType = fieldValue.GetType();
            var attrValueType  = attr.Value.GetType();

            var currentEnumValue  = Enum.Parse(fieldValueType, fieldValue.ToString());
            var expectedEnumValue = Enum.Parse(fieldValueType, attr.Value.ToString());

            if (!invertedScope) {
                return !currentEnumValue.Equals(expectedEnumValue);
            }

            return currentEnumValue.Equals(expectedEnumValue);
        }


        /// <summary>
        /// Gets the object the property represents.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static object GetTargetObjectOfProperty(SerializedProperty property)
        {
            if (property == null)
            {
                return null;
            }

            string   path     = property.propertyPath.Replace(".Array.data[", "[");
            object   obj      = property.serializedObject.targetObject;
            string[] elements = path.Split('.');

            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    string elementName = element.Substring(0, element.IndexOf("["));
                    int index = Convert.ToInt32(element.Substring(element.IndexOf("["))
                        .Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }

            return obj;
        }

        /// <summary>
        /// Gets the object that the property is a member of
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static object GetTargetObjectWithProperty(SerializedProperty property)
        {
            var path = property.propertyPath.Replace(".Array.data[", "[");

            object obj      = property.serializedObject.targetObject;
            var    elements = path.Split('.');
        
            for (int i = 0; i < elements.Length - 1; i++)
            {
                string element = elements[i];
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index       = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }

            return obj;
        }

        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
            {
                return null;
            }

            var type = source.GetType();

            while (type != null)
            {
                var field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    return field.GetValue(source);
                }

                var property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property != null)
                {
                    return property.GetValue(source, null);
                }

                type = type.BaseType;
            }

            return null;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            if (!(GetValue_Imp(source, name) is IEnumerable enumerable))
            {
                return null;
            }

            var enumerator = enumerable.GetEnumerator();
            for (var i = 0; i <= index; i++)
            {
                if (!enumerator.MoveNext())
                {
                    return null;
                }
            }

            return enumerator.Current;
        }
        #endregion

        /*public static void OnDataChanged(SerializedProperty property) {
            property.serializedObject.ApplyModifiedProperties();

            var attr = TryGetAttributes<OnValueChangedAttribute>(property);

            if (attr.Length == 0)
                return;

            var target = GetTargetObjectWithProperty(property);

            foreach (var obj in attr) {
                var method = target.TryGetMethod(obj.callbackMethod);

                if (method != null && method.GetParameters().Length == 0 && method.ReturnType == typeof(void)) {
                    method.Invoke(target, new object[] { });
                }

                else {
                    Debug.LogError($"Can't find any method with name {obj.callbackMethod} and return type 'void'.");
                }
            }
        }*/

        /*public static GUIContent GetGUIContent(SerializedProperty property) {
            var label = TryGetAttribute<HideLabelAttribute>(property) == null ? 
                $"{property.displayName}" : string.Empty;
            
            if(string.IsNullOrEmpty(label))
                return GUIContent.none;
            
            var content = new GUIContent(label);
            var tooltip = TryGetAttribute<PropertyTooltipAttribute>(property);
            if (tooltip != null) {
                content.tooltip = tooltip.Text;
            }

            return null; // content;
        }*/
        
        public static bool IsUnitySerialized(this FieldInfo fieldInfo) {
            if (fieldInfo.IsDefined(typeof(NonSerializedAttribute)))
                return false;
            
            if (fieldInfo.IsPublic)
                return true;

            if (!fieldInfo.IsPrivate && !fieldInfo.IsFamilyOrAssembly) 
                return false;

            return fieldInfo.IsDefined(typeof(SerializableAttribute));
        }

        public static bool IsBuiltIn(Type objType) {
            if (objType == null)
                return false;
            
            if (objType.IsPrimitive || objType == typeof(string) || objType == typeof(void))
                return true;

            if (typeof(MonoBehaviour).IsAssignableFrom(objType)) {
                return true;
            }
            
            if (objType == typeof(bool))
            {
                return true;
            }
            if (objType == typeof(int))
            {
                return true;
            }
            if (objType == typeof(long))
            {
                return true;
            }
            if (objType == typeof(float))
            {
                return true;
            }
            if (objType == typeof(double))
            {
                return true;
            }
            if (objType == typeof(string))
            {
                return true;
            }
            if (objType == typeof(Vector2))
            {
                return true;
            }
            if (objType == typeof(Vector3))
            {
                return true;
            }
            if (objType == typeof(Vector4))
            {
                return true;
            }
            if (objType == typeof(Color))
            {
                return true;
            }
            if (objType == typeof(Bounds))
            {
                return true;
            }
            if (objType == typeof(Rect)) {
                return true;
            }
            if (typeof(Object).IsAssignableFrom(objType))
            {
                return true;
            }
            if (typeof(Enum).IsAssignableFrom(objType))
            {
                return true;
            }
            if (typeof(TypeInfo).IsAssignableFrom(objType))
            {
                return true;
            }
            
            return false;
        }

        public static object GetTargetObject(object target, MemberInfo info) {
            if (info is PropertyInfo propertyInfo) {
                return propertyInfo.GetValue(target);
            }

            if (info is FieldInfo fieldInfo) {
                return fieldInfo.GetValue(target);
            }

            return null;
        }
        
        public static void SetTargetValue(FriggProperty property, object target, MemberInfo info, object value) {
            
            if (info is PropertyInfo propertyInfo) { 
                if(propertyInfo.CanWrite)
                   propertyInfo.SetValue(target, value);
            }

            if (info is FieldInfo fieldInfo) {
                 fieldInfo.SetValue(target, value);
            }
        }
    }

    public class PropertyValue {
        public PropertyMeta MetaInfo { get; set; }
        public object       target   { get; set; }
    }
}
