namespace Assets.Scripts.Tests {
    using System.Collections.Generic;
    using Attributes;
    using UnityEngine;
    using Utils;

    public class DropdownBehaviour : MonoBehaviour {
        
        [Dropdown("intValues")]
        public int intValue;

        private int[] intValues = {1, 2, 3, 4, 5};

        [Dropdown("VectorValues")]
        public Vector3 vectorValue;
        
        [Dropdown("StringValues")]
        public string stringValue;
        
        private List<string> StringValues { get { return new List<string>() { "A", "B", "C", "D", "E" }; } }

        private DropdownList<Vector3> VectorValues() => 
            new DropdownList<Vector3>() {
                {"test", Vector3.back},
                {"testtwo", Vector3.up},
                {"testthree", Vector3.forward},
                {"testfour", Vector3.left}
            };
    }
}