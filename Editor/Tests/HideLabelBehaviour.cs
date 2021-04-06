using Assets.Scripts.Attributes.Meta;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    using System.Collections.Generic;
    using Attributes;

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