namespace Frigg.Tests {
    using UnityEngine;

    public class RequiredBehaviour : MonoBehaviour {
        [Required]
        public GameObject testObject;
        
        [Required("I am a custom dude")]
        public Transform testTransform;

        [Required]
        public int testNonRef = 0;

        [Required("This field can not be 0!")]
        public float floatNonRef;
    }
}