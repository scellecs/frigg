namespace Assets.Scripts.Tests {
    using Packages.Frigg.Attributes;
    using UnityEngine;

    public class InfoBoxBehaviour : MonoBehaviour {
        [Title("InfoBox attribute")]
        [InfoBox("Test box type Info")]
        public int testInteger;
        
        [InfoBox("Test box type Warning & fixed height", InfoMessageType = InfoMessageType.Warning, Height = 15)]
        public int testIntegerTwo;
        
        [InfoBox("Test box type Error & custom font size", InfoMessageType = InfoMessageType.Error, FontSize = 16)]
        public int testIntegerThree;
    }
}