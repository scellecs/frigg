namespace Frigg.Editor {
    using System.Linq;
    using Packages.Frigg.Editor.Utils;
    using UnityEditor;
    using Utils;
    using Object = UnityEngine.Object;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true)]
    public class FriggInspector : Editor {
        private PropertyTree propertyTree;
        public  PropertyTree PropertyTree => this.propertyTree;
        
        private void OnEnable() {
            //EditorData.Erase();
            this.propertyTree = PropertyTree.InitTree(this.serializedObject);
        }

        public override void OnInspectorGUI() {
            //Here we are drawing our inspector.
            GuiUtilities.DrawTree(this.PropertyTree);
        }
    }
} 