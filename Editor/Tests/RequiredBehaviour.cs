namespace Frigg.Tests {
    using UnityEngine;

    public class RequiredBehaviour : MonoBehaviour {
        [Required]
        [InfoBox("Wassup")]
        public GameObject testObject;
        
        [Required("I am a custom dude")]
        public Transform testTransform;
    }
}