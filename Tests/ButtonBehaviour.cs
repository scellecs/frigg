namespace Frigg.Tests {
    using UnityEngine;

    public class ButtonBehaviour : MonoBehaviour {

        [Button, Order(1)]
        public void TestMethodOne() {
            Debug.Log("Hi there! I am second ordered button");
        }
        
        [Button("Button with a custom name")]
        public void TestMethodTwo() {
            Debug.Log("Hi there! (custom)");
        }
    }
}