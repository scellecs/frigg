namespace Frigg.Tests {
    using UnityEngine;

    public class BuiltInTest : MonoBehaviour {
        public string someString;

        [PropertySpace(10)]
        [Title("123")]
        [Readonly]
        [InfoBox("Some infobox")]
        public int someInt;

        public int someOtherInt;

        [PropertySpace(10)]
        [Title("123")]
        public string thenSomeOtherStr;

        [Header("Tools")]
        [Required]
        public TestMonoObject testMonoObject;
    }
}