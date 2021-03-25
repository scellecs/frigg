using Assets.Scripts.Attributes.Meta;
using UnityEngine;

namespace Assets.Scripts.Tests
{
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
    }
}