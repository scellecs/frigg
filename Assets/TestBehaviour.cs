namespace Assets {
    using System.Collections.Generic;
    using Scripts.Attributes;
    using Scripts.Utils;
    using UnityEngine;

    public class TestBehaviour : MonoBehaviour {

        [Dropdown("intValues")]
        public int intValue;

        private int[] intValues = {1, 2, 3, 4, 5};

        [Dropdown("vectorValues")]
        private Vector3 vectorValue;

        private DropdownList<string> vectorValues = new DropdownList<string>() {
            {"test", "Testcaseone"},
            {"testtwo", "Testcasetwo"},
            {"testthree", "Testcasethree"},
            {"testfour", "Testcasefour"}
        };
            
        [Button("Simple button to click one")]
        public void SimpleLog() {
            Debug.Log("Simple log");
        }
        
        [Button]
        public void SimpleLogWithoutName() {
            Debug.Log("Simple log two");
        }
    }
}