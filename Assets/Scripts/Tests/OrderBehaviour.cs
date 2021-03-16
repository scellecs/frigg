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
        
        [Button("Ordered 1"), Order(1)]
        public void SimpleOne() {
            Debug.Log("Simple log");
        }
        
        [Button("Ordered 0"), Order]
        public void TestTwo() {
            Debug.Log("Simple log two");
        }
        
        [Button("Ordered 0 too"), Order]
        public void TestThree() {
            Debug.Log("Simple log two");
        }

        [Order(5)]
        [ShowInInspector]
        private int TestToEdit;
        
        [Order(4)]
        [ShowInInspector]
        public int TestProperty { get; set;}
    }
}