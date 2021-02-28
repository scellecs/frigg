namespace Assets.Scripts.Utils {
    using System.Reflection;
    using Attributes;
    using UnityEditor;
    using UnityEngine;

    public class GuiUtilities {
        public static void Button(Object obj, MethodInfo info) {
            var attr = (ButtonAttribute)info.GetCustomAttributes(typeof(ButtonAttribute), true)[0];
            
            if (GUILayout.Button(attr.Text)) {
                info.Invoke(obj, new object[]{ });
            }
        }

        public static void Dropdown(Object obj, FieldInfo info) {
            var attr = (DropdownAttribute) info.GetCustomAttributes(typeof(DropdownAttribute), true)[0];
            //Todo: check & refactor
        }
    }
}