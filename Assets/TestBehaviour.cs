namespace Assets {
    using System.Collections.Generic;
    using Scripts.Attributes;
    using UnityEngine;

    public class TestBehaviour : MonoBehaviour {

        [Dropdown("intValues")]
        public int intValue;

        private int[] intValues = {1, 2, 3, 4, 5}; 

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