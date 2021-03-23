namespace Assets.Scripts.Tests {
    using Attributes;
    using Attributes.Custom;
    using Attributes.Meta;
    using UnityEngine;

    public class OrderBehaviour : MonoBehaviour {

        [Readonly, Order(3)]
        public Vector3 data = Vector3.back;
        
        [Order(2)]
        public int value;
        
        [Button("Ordered 2"), Order(2)]
        public void SimpleTwo() {
            Debug.Log("Simple ordered 2");
        }
        
        [Button("Ordered 3"), Order(3)]
        public void SimpleThree() {
            Debug.Log("Simple ordered 3");
        }
        
        [Button("Ordered 1"), Order(1)]
        public void SimpleOne() {
            Debug.Log("Simple ordered 1");
        }
        
        [Order(4)]
        [ShowInInspector]
        private int testToEdit;
        
        [Button("Ordered 5"), Order(5)]
        public void SimpleFive() {
            Debug.Log("Simple ordered 5");
        }
        
        [Order]
        [ShowInInspector]
        public int TestProperty { get; set;}
    }
}