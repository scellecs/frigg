namespace Assets.Scripts.Tests {
    using System;
    using Attributes;
    using Attributes.Custom;
    using Attributes.Meta;
    using UnityEngine;

    [HideMonoScript]
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

        [InlineProperty]
        [PropertyTooltip("Inline stuff")]
        public SomeSerializable some;
        
        [Serializable]
        public struct SomeSerializable {
            public int one;
            public int two;
        }
    }
}