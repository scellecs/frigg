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

        public static void TryGetMembers(this object target, List<Member> info) {
            if (target != null) {
                //get type of provided target
                var    type = target.GetType();

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

                        var member = new Member {memberInfo = method, target = target};
                        if (!info.Contains(member)) {
                            info.Add(member);
                        }
                    }
                    
                    foreach (var field in fields) {
                        if ((!type.IsDefined(typeof(SerializableAttribute))
                             || !field.IsDefined(typeof(SerializableAttribute)) && !field.IsPublic) 
                            && !field.IsDefined(typeof(ShowInInspectorAttribute))) {
                            continue;
                        }

                        var member = new Member {memberInfo = field, target = target};
                        if (!info.Contains(member)) {
                            info.Add(member);
                        }
                    }

                    foreach (var property in properties) {
                        if (!property.IsDefined(typeof(SerializableAttribute)) 
                            && !property.IsDefined(typeof(ShowInInspectorAttribute))) {
                            continue;
                        }

                        var member = new Member {memberInfo = property, target = target};
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

        public static MethodInfo TryGetMethod(this object target, string name) {
            if(target != null)
               return  target.TryGetMethods(x => x.Name == name).FirstOrDefault();

            Debug.LogError("There's no target specified.");
            return null;
        }
    
        public static FieldInfo TryGetField(this object target, string name) {
            if (target != null)
                return target.GetType().GetField("name");

            Debug.LogError("There's no target specified.");
            return null;
        }
    
        public static PropertyInfo TryGetProperty(this object target, string name) {
            if (target != null)
                return target.GetType().GetProperty("name");

            Debug.LogError("There's no target specified.");
            return null;
        }
    
        public static Type TryGetListElementType(Type listType) 
            => listType.IsGenericType ? listType.GetGenericArguments()[0] : listType.GetElementType();

        #endregion

        #region properties //TODO: Check for any kind of errors.
    
        public static Type GetPropertyType(SerializedProperty property)
        {
            var parentType = GetTargetObjectWithProperty(property).GetType();
            var fieldInfo  = parentType.GetField(property.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            return fieldInfo?.FieldType;
        }

        public static bool IsPropertyVisible(SerializedProperty property) {
            ValidatorAttribute attr = TryGetAttribute<HideIfAttribute>(property);

            if (attr != null) {
                return GetConditionValue(property, attr, false);
            }

            attr = TryGetAttribute<ShowIfAttribute>(property);
            return attr == null || GetConditionValue(property, attr, true);
        }

        public static bool IsPropertyEnabled(SerializedProperty property) {
            ValidatorAttribute attr = TryGetAttribute<DisableIfAttribute>(property);

            if (attr != null) {
                return GetConditionValue(property, attr, false);
            }

            attr = TryGetAttribute<EnableIfAttribute>(property);
            return attr == null || GetConditionValue(property, attr, true);
        }

        private static bool GetConditionValue(SerializedProperty property, ValidatorAttribute attr, bool invertedScope) {
            var target = GetTargetObjectWithProperty(property);
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

            if (fieldValueType != attrValueType) {
                var infoBoxParams = new InfoBoxAttribute(
                    $"'{attr.FieldName}' type is not the same Type as {fieldValueType}.") {
                    InfoMessageType = InfoMessageType.Error
                };
                
                var infoBox = DecoratorDrawerUtils.GetDecorator(typeof(InfoBoxAttribute));
                infoBox.OnGUI(EditorGUILayout.GetControlRect(true, 0),
                    property, infoBoxParams);

                return false;
            }

            var currentEnumValue  = Enum.Parse(fieldValueType, fieldValue.ToString());
            var expectedEnumValue = Enum.Parse(fieldValueType, attr.Value.ToString());

            if (!invertedScope) {
                return !currentEnumValue.Equals(expectedEnumValue);
            }

            return currentEnumValue.Equals(expectedEnumValue);
        }

        public static T[] TryGetAttributes<T>(SerializedProperty property) where T : class {
            var target = GetTargetObjectWithProperty(property);
            
            var fInfo  = target.TryGetField(property.name);

            if (fInfo == null) {
                return new T[] { };
            }

            var data = (T[]) fInfo.GetCustomAttributes(typeof(T), true);

            return data;
        }

        public static T TryGetAttribute<T>(SerializedProperty property) where T : class {
            var attr = TryGetAttributes<T>(property);
            
            return (attr.Length > 0) ? attr[0] : null;
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
        
        public static void SetDefaultValue(SerializedProperty baseProperty, SerializedProperty element) {

            var copy = element.Copy();
            
            do {
                var type = copy.propertyType;

                switch (type) {
                    case SerializedPropertyType.Boolean:
                        copy.boolValue = default;
                        break;
                    case SerializedPropertyType.Integer:
                        copy.longValue = default;
                        copy.intValue  = default;
                        break;
                    case SerializedPropertyType.Float:
                        copy.floatValue  = default;
                        copy.doubleValue = default;
                        break;
                    case SerializedPropertyType.String:
                        copy.stringValue = default;
                        break;
                    case SerializedPropertyType.Vector2:
                        copy.vector2Value = default;
                        break;
                    case SerializedPropertyType.Vector3:
                        copy.vector3Value = default;
                        break;
                    case SerializedPropertyType.Vector4:
                        copy.vector4Value = default;
                        break;
                }
            } while (copy.NextVisible(true));
        }
        
        public static bool HasDefaultValue(SerializedProperty property, Type type) {
            if (type == typeof(bool)) {
                return property.boolValue == default;
            }
            if (type == typeof(int))
            {
                return property.intValue == default;
            }
            if (type == typeof(long))
            {
                return property.longValue == default;
            }
            if (type == typeof(float))
            {
                return property.floatValue == default;
            }
            if (type == typeof(double))
            {
                return property.doubleValue == default;
            }
            if (type == typeof(string))
            {
                return property.stringValue == default;
            }
            if (type == typeof(Vector2))
            {
                return property.vector2Value == default;
            }
            if (type == typeof(Vector3))
            {
                return property.vector3Value == default;
            }
            if (type == typeof(Vector4))
            {
                return property.vector4Value == default;
            }
            if (type == typeof(Color))
            {
                return property.colorValue == default;
            }
            if (type == typeof(Bounds))
            {
                return property.boundsValue == default;
            }
            if (type == typeof(Rect))
            {
                return property.rectValue == default;
            }

            return true;
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

        public static void OnDataChanged(SerializedProperty property) {
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
        }


        public static GUIContent GetGUIContent(MemberInfo property) {
            var niceName = ObjectNames.NicifyVariableName(property.Name);
            var label = !property.IsDefined(typeof(HideLabelAttribute)) ? 
                $"{niceName}" : string.Empty;
            
            var content = new GUIContent(label);
            var isDefined = property.IsDefined(typeof(PropertyTooltipAttribute));
            if (!isDefined) {
                return content;
            }

            var toolTip = property.GetCustomAttribute<PropertyTooltipAttribute>();
            content.tooltip = toolTip.Text;

            return content;
        }
        
        public static GUIContent GetGUIContent(SerializedProperty property) {
            var label = TryGetAttribute<HideLabelAttribute>(property) == null ? 
                $"{property.displayName}" : string.Empty;
            
            if(string.IsNullOrEmpty(label))
                return GUIContent.none;
            
            var content = new GUIContent(label);
            var tooltip = TryGetAttribute<PropertyTooltipAttribute>(property);
            if (tooltip != null) {
                content.tooltip = tooltip.Text;
            }

            return content;
        }
        
        public static bool IsUnitySerialized(this FieldInfo fieldInfo) {
            if (fieldInfo.IsDefined(typeof(NonSerializedAttribute)))
                return false;
            
            if (fieldInfo.IsPublic)
                return true;

            if (!fieldInfo.IsPrivate && !fieldInfo.IsFamilyOrAssembly) 
                return false;

            return fieldInfo.IsDefined(typeof(SerializableAttribute));
        }

        public static bool IsPrimitiveUnityType(Type objType) {
            if (objType == null)
                return false;
            
            if (objType.IsPrimitive || objType == typeof(string))
                return true;
            
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

    }
}
