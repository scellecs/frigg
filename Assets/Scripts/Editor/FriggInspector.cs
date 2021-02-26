namespace Assets.Scripts.Editor {
    using System.Collections.Generic;
    using System.Reflection;
    using Attributes;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Utils;
    
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true)]
    public class FriggInspector : Editor {

        private IEnumerable<MethodInfo> methods;

        public override VisualElement CreateInspectorGUI() => base.CreateInspectorGUI();

        public override void OnInspectorGUI() {
            this.methods = target.TryGetMethods(info
                => info.GetCustomAttributes(typeof(ButtonAttribute), true).Length > 0);
            
            this.DrawButtons();
        }

        protected void DrawButtons() {
            foreach (var element in this.methods) {
                GuiUtilities.Button(this.serializedObject.targetObject, element);
            }
        }
    }
}