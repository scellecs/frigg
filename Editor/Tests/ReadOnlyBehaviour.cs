namespace Frigg.Tests {
    using System.Collections.Generic;
    using UnityEngine;

    public class ReadOnlyBehaviour : MonoBehaviour {
        
        [Readonly]
        public Vector3 TestVector;

        [Readonly]
        public int testInt = 123;

        [Readonly]
        public string testString = "test string data";

        [Readonly]
        public List<string> readOnlyArray;
    }
}