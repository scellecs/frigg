namespace Frigg.Tests {
    using UnityEngine;

    public class ShowInInspectorBehaviour : MonoBehaviour {
        
        public int publicField;
        
        [ShowInInspector]
        protected int protectedVisible;

        protected int protectedInvisible;

        [ShowInInspector]
        internal int internalVisible;
        
        internal int internalInvisible;

        private int privateInvisible;

        [ShowInInspector]
        public int publicFieldAttr;

        [ShowInInspector]
        public string publicString = "test string";

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