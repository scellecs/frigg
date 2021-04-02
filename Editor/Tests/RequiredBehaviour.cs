namespace Assets.Scripts.Tests {
    using Packages.Frigg.Attributes;
    using UnityEngine;

    public class RequiredBehaviour : MonoBehaviour {
        [Required]
        [InfoBox("Wassup")]
        public GameObject testObject;
        
        [Required("I am a custom dude")]
        public Transform testTransform;
    }
}