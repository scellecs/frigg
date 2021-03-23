namespace Assets.Scripts.Tests {
    using System.Collections.Generic;
    using Attributes;
    using UnityEngine;

    public class ReadOnlyBehaviour : MonoBehaviour {
        
        [Readonly]
        public Vector3 data = Vector3.back;

        [Readonly]
        public int testInt = 123;
        
        [Readonly]
        public string testString = "test string data";

        [Readonly]
        public List<string> readOnlyArray;
    }
}