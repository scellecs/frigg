namespace Frigg.Tests {
    using UnityEngine;

    public class OnValueChangedBehaviour : MonoBehaviour {

        [OnValueChanged(nameof(OnChangedTestMethod))]
        public int testValue;

        public void OnChangedTestMethod() {
            Debug.Log($"{nameof(this.testValue)} has changed. Now it's {this.testValue}.");
        }
    }
}