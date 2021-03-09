namespace Assets {
    using System;
    using System.Collections.Generic;
    using Scripts.Attributes;
    using Scripts.Attributes.Custom;
    using Scripts.Utils;
    using UnityEngine;

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

        #region button
        [Button("Simple button to click one")]
        public void SimpleLog() {
            Debug.Log("Simple log");
        }
        
        [Button]
        public void SimpleLogWithoutName() {
            Debug.Log("Simple log two");
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
    }
}