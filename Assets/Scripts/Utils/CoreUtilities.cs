using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class CoreUtilities
{
    #region reflection
    public static IEnumerable<MethodInfo> TryGetMethods(this object target, Func<MethodInfo, bool> predicate) {
        if (target != null) {
            var data = target.GetType()
                .GetMethods(BindingFlags.Instance 
                            | BindingFlags.Static
                            | BindingFlags.NonPublic 
                            | BindingFlags.Public).Where(predicate);

            return data;
        }

        Debug.LogError("There's no target specified.");
        return null;
    }

    public static IEnumerable<FieldInfo> TryGetFields(this object target, Func<FieldInfo, bool> predicate) {
        if (target != null) {
            var data = target.GetType()
                .GetFields(BindingFlags.Instance 
                           | BindingFlags.Static
                           | BindingFlags.NonPublic 
                           | BindingFlags.Public).Where(predicate);
            
            return data;
        }
        
        Debug.LogError("There's no target specified.");
        return null;
    }
    
    public static IEnumerable<PropertyInfo> TryGetProperties(this object target, Func<PropertyInfo, bool> predicate) {
        if (target != null) {
            var data = target.GetType()
                .GetProperties(BindingFlags.Instance 
                           | BindingFlags.Static
                           | BindingFlags.NonPublic 
                           | BindingFlags.Public).Where(predicate);
            
            return data;
        }
        
        Debug.LogError("There's no target specified.");
        return null;
    }
    
    public static MethodInfo TryGetMethod(this object target, string name) {
        if (target != null) {
            return target.TryGetMethods(x => x.Name == name).FirstOrDefault();
        }
        
        Debug.LogError("There's no target specified.");
        return null;
    }
    
    public static FieldInfo TryGetField(this object target, string name) {
        if (target != null) {
            return target.TryGetFields(x => x.Name == name).FirstOrDefault();
        }
        
        Debug.LogError("There's no target specified.");
        return null;
    }
    
    public static PropertyInfo TryGetProperty(this object target, string name) {
        if (target != null) {
            return target.TryGetProperties(x => x.Name == name).FirstOrDefault();
        }
        
        Debug.LogError("There's no target specified.");
        return null;
    }
    #endregion

    #region properties //TODO: Check for any kind of errors.

    public static T[] TryGetAttributes<T>(SerializedProperty property) where T : class {
        var fInfo = GetTargetObjectWithProperty(property).TryGetField(property.name);

        if (fInfo == null)
        {
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
                int    index       = Convert.ToInt32(element.Substring(element.IndexOf("["))
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
        var   path        = property.propertyPath.Replace(".Array.data[", "[");

        object   obj      = property.serializedObject.targetObject;
        var elements      = path.Split('.');
        
        for (int i = 0; i < elements.Length - 1; i++)
        {
            string element = elements[i];
            if (element.Contains("["))
            {
                var elementName = element.Substring(0, element.IndexOf("["));
                var    index       = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
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
        var enumerable = GetValue_Imp(source, name) as IEnumerable;
        if (enumerable == null)
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
}
