namespace Frigg.Tests {
    using System;
    using UnityEngine;

    [HideMonoScript]
    public class PropertyTooltipBehaviour : MonoBehaviour {
        [PropertyTooltip("It's private non serialized field", false)]
        [ShowInInspector]
        private int test;

        [PropertyTooltip("It's public field", false)]
        public int publicTest;

        [PropertyTooltip("It's property", false)]
        [ShowInInspector]
        public int PropertyTest { get; set; }

        [PropertyTooltip("It's button", false)]
        [Button]
        public void ButtonTest() {
            
        }

        [InlineProperty]
        [PropertyTooltip("Inline stuff", false)]
        public SomeSerializable some;
        
        [Serializable]
        public struct SomeSerializable {
            public int one;
            public int two;
        }
    }
}