namespace Assets {
    using System.Collections.Generic;
    using Scripts.Attributes;
    using UnityEngine;

    public class TestBehaviour : MonoBehaviour {

        [Button("Simple button to click")]
        public void SimpleLog() {
            Debug.Log("Simple log");
        }
        
        [Button]
        public void SimpleLogWithoutName() {
            Debug.Log("Simple log2");
        }
    }
}