namespace Frigg.Tests {
    using UnityEngine;

    [HideMonoScript]
    public class ValidatorBehaviour : MonoBehaviour {
        public TestEnum someEnum = TestEnum.TestShow;

        public bool someToggle;

        [HideIf("someToggle", true)]
        public int visibleWhenFalse;
        
        /*[HideIf("someEnum", TestEnum.TestHide)]
        public int hideIfEnumHide;*/
        
        [ShowIf("someToggle", true)]
        public int visibleWhenTrue;
        
        /*[ShowIf("someEnum", TestEnum.TestShow)]
        public int showIfEnumShow;*/

        [DisableIf("someToggle", true)]
        public int disabledWhenTrue;

        [EnableIf("someToggle", true)]
        public int enabledWhenTrue;
    }

    public enum TestEnum {
        TestShow = 0,
        TestHide = 1
    }
}