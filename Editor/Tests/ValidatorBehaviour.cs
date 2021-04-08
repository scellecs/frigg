namespace Assets.Scripts.Tests {
    using Attributes.Meta;
    using Packages.Frigg.Attributes.Validators;
    using UnityEngine;

    [HideMonoScript]
    public class ValidatorBehaviour : MonoBehaviour {
        public TestEnum someEnum = TestEnum.TestShow;

        public bool someToggle;

        [HideIf("someToggle")]
        public int hideIfToggleTrue;
        
        [HideIf("someEnum", TestEnum.TestHide)]
        public int hideIfEnumHide;
        
        [ShowIf("someToggle")]
        public int showIfToggleTrue;
        
        [ShowIf("someEnum", TestEnum.TestShow)]
        public int showIfEnumShow;

        [DisableIf("someToggle")]
        public int disableIfToggleTrue;

        [EnableIf("someToggle")]
        public int enableIfToggleTrue;
    }

    public enum TestEnum {
        TestShow = 0,
        TestHide = 1
    }
}