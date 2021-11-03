namespace Frigg.Editor {
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using UnityEngine;
    using Utils;
    using Object = UnityEngine.Object;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true)]
    public class FriggInspector : Editor {
        private PropertyTree propertyTree;
        public  PropertyTree PropertyTree => this.propertyTree;
        
        private void OnEnable() {
            this.propertyTree = PropertyTree.InitTree(this.serializedObject);
        }

        public override void OnInspectorGUI() {
            this.serializedObject.Update();
            //Here we are drawing our inspector.
            GuiUtilities.DrawTree(this.PropertyTree);
            
            this.Repaint();
        }
    }
} 