namespace Assets {
    using System.Collections.Generic;
    using Scripts.Attributes;
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
        public enum TestFlags {
            One = 0,
            Two = 1,
            Three = 3,
            Four = 4 
        }

        [EnumFlags]
        public TestFlags flags;
        #endregion

    }
}