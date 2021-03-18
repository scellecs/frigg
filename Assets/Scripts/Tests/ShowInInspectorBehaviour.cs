namespace Assets.Scripts.Tests {
    using Attributes.Custom;
    using Attributes.Meta;
    using UnityEngine;

    public class ShowInInspectorBehaviour : MonoBehaviour {
        
        public int publicField;

        [ShowInInspector]
        public int publicFieldAttr;

        [ShowInInspector]
        public string publicString;

        [ShowInInspector]
        public bool publicBool;
        
        [ShowInInspector]
        private int privateField;
        
        [ShowInInspector]
        private bool privateBool;
        
        [ShowInInspector]
        private string privateString;
        
        [ShowInInspector]
        public int ReadOnlyPropertyWithAttr => this.privateField;

        [ShowInInspector]
        public int AnotherCustomProperty {
            get => this.publicFieldAttr;
            set => this.publicFieldAttr = value; 
        }
    }
}