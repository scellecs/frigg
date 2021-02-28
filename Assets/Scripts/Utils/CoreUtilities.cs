using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class CoreUtilities
{
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
}
