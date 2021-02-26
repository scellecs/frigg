namespace Assets.Scripts.Utils {
    using System.Reflection;
    using Attributes;
    using UnityEngine;

    public class GuiUtilities {
        public static void Button(Object obj, MethodInfo info) {
            var attr = (ButtonAttribute)info.GetCustomAttributes(typeof(ButtonAttribute), true)[0];

            if (GUILayout.Button(attr.Text)) {
                
            }
        }
    }
}