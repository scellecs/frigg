namespace Frigg.Tests {
    using UnityEngine;

    public class InfoBoxBehaviour : MonoBehaviour {
        [Title("InfoBox attribute")]
        [InfoBox("Test box type Info")]
        public int testInteger;
        
        [InfoBox("Test WarningBox & fixed height (wont resize)", InfoMessageType = InfoMessageType.Warning, Height = 60)]
        public int testIntegerTwo;
        
        [InfoBox("Test box type Error & custom font size", InfoMessageType = InfoMessageType.Error, FontSize = 16)]
        public int testIntegerThree;

        [InfoBox("Property InfoBox")]
        [ShowInInspector]
        public int Property { get; set; }

        [InfoBox("Button InfoBox")]
        [Button("Test button")]
        [Order(1)]
        public void TestButton() {
        }
    }
}