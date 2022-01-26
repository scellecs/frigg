namespace Frigg {
    public class HideIfAttribute : ConditionAttribute {
        public HideIfAttribute(string name) : base(name) {
        }

        public HideIfAttribute(string name, object expected) : base(name, expected) {
        }
    }
}