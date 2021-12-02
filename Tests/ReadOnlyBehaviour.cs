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

        [Title("Writable")]
        public List<string> readOnlyArray;

        [Title("Readonly")]
        [Readonly]
        public List<int> someReadOnlyList = new List<int>() {
            1, 2, 3, 4, 5, 6
        };
    }
}