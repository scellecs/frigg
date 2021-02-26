namespace Assets {
    using System.Collections.Generic;
    using Scripts.Attributes;
    using UnityEngine;

    public class TestBehaviour : MonoBehaviour {

        [Button("Simple button to click")]
        public void SimpleLog() {
        }
        
        [Button]
        public void SimpleLogWithoutName() {
        }
    }
}