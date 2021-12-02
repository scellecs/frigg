using UnityEngine;

namespace Frigg.Tests {
    using System.Collections.Generic;

    public class HideLabelBehaviour : MonoBehaviour {
        
        [HideLabel]
        public Vector3 someVector;

        [HideLabel]
        public int someInteger;

        [HideLabel]
        public string someString;
        
        [ShowInInspector]
        [HideLabel]
        public int someProperty { get; set; }

        [ShowInInspector]
        [HideLabel]
        private int somePrivateField;
        
        [Dropdown("StringValues")]
        [HideLabel]
        public string stringValue;
        
        private List<string> StringValues { get { return new List<string>() { "A", "B", "C", "D", "E" }; } }
    }
}