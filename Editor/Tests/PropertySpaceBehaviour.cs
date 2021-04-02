namespace Assets.Scripts.Tests {
    using Attributes;
    using Attributes.Meta;
    using Packages.Frigg.Attributes;
    using UnityEngine;

    public class PropertySpaceBehaviour : MonoBehaviour {
        
        [ShowInInspector]
        private int testInteg;

        [PropertySpace(SpaceBefore = 20), Order(1)]
        public float testFloat;

        [PropertySpace(SpaceBefore = 10)]
        [Button, Order(1)]
        public void ClickMe() {
            
        }
    }
}