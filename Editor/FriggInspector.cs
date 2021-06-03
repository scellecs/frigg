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
            //Init tree with 'this' script.
        }

        public override void OnInspectorGUI() {
            this.serializedObject.Update();
            GuiUtilities.DrawTree(this.PropertyTree);
            PropertyTree.UpdateTree();
        }
    }
} 