namespace Assets.Scripts.Tests {
    using System;
    using System.Collections.Generic;
    using Attributes;
    using Attributes.Custom;
    using Attributes.Meta;
    using UnityEngine;
    using Utils;

    public class TestBehaviour : MonoBehaviour {

        #region dropdown
        [Dropdown("intValues")]
        public int intValue;

        private int[] intValues = {1, 2, 3, 4, 5};

        [Dropdown("VectorValues")]
        public Vector3 vectorValue;
        
        [Dropdown("StringValues")]
        public string stringValue;
        
        private List<string> StringValues { get { return new List<string>() { "A", "B", "C", "D", "E" }; } }

        private DropdownList<Vector3> VectorValues() {
            return new DropdownList<Vector3>() {
                {"test", Vector3.back},
                {"testtwo", Vector3.up},
                {"testthree", Vector3.forward},
                {"testfour", Vector3.left}
            };
        }
        #endregion
        
        #region enum flags
        
        [Flags]
        public enum TestFlags {
            None  = 0,
            One   = 1 << 0,
            Two   = 1 << 1,
            Three = 1 << 2,
            Four  = 1 << 3
        }

        [EnumFlags]
        public TestFlags flags;
        
        [EnumFlags("Enum flags with name")]
        public TestFlags flagsTwo;

        #endregion
        
        #region reorderableList

        [ReorderableList]
        public int[] intArray;

        [ReorderableList]
        public List<int> intlist;
        
        [ReorderableList]
        public float[] floatArray;
        
        [ReorderableList]
        public List<float> floatlist;
        
        [ReorderableList]
        public Vector3[] vectorArray;
        
        [ReorderableList]
        public List<Vector3> vectorlist;

        #endregion

        #region readonly

        [Readonly]
        public Vector3 data = Vector3.back;

        [Readonly]
        public int testInt = 123;
        
        [Readonly]
        public string testString = "test string data";

        #endregion

        #region showInInspector

        public int JustATest;

        [ShowInInspector]
        private int privateInt;

        [ShowInInspector]
        private string privateString;
        
        [ShowInInspector]
        public int publicInt;
        
        [ShowInInspector]
        public string publicString;

        [ShowInInspector]
        public bool publicBool;
        
        [ShowInInspector]
        private bool privateBool;
        
        [ShowInInspector]
        public int ReadOnlyProperty => this.privateInt;

        [ShowInInspector]
        public int AnotherCustomProperty {
            get => this.publicInt;
            set => this.publicInt = value; 
        }

        #endregion
    }
}