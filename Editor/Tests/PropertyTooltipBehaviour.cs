namespace Assets.Scripts.Tests {
    using Attributes;
    using Attributes.Meta;
    using UnityEngine;

    public class PropertyTooltipBehaviour : MonoBehaviour {
        [PropertyTooltip("It's private non serialized field")]
        [ShowInInspector]
        private int test;

        [PropertyTooltip("It's public field")]
        public int publicTest;

        [PropertyTooltip("It's property")]
        [ShowInInspector]
        public int PropertyTest { get; set; }

        [PropertyTooltip("It's button")]
        [Button]
        public void ButtonTest() {
            
        }
    }
}