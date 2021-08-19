namespace Frigg.Editor {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true)]
    public class FriggInspector : Editor {
        private PropertyTree propertyTree;
        public  PropertyTree PropertyTree => this.propertyTree;

        private bool anySerialized;
        private bool hasArrays;

        private ILookup<int, object> mixedData;
        private void OnEnable() {
            //EditorData.Erase();
            this.propertyTree = PropertyTree.InitTree(this.serializedObject);
        }

        public override void OnInspectorGUI() {
            //Update SO representation.
            this.serializedObject.Update();
            
            //Here we are drawing our inspector.
            GuiUtilities.DrawTree(this.PropertyTree);
            
            //We need to call this each time we update our inspector to update entire tree and get newest values.
            this.PropertyTree.UpdateTree();
        }
    }
} 