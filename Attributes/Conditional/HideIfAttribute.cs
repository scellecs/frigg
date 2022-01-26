namespace Frigg {
    public class HideIfAttribute : ConditionAttribute {
        public HideIfAttribute(string expression) : base(expression) {
        }

        public HideIfAttribute(string name, bool expected) : base(name, expected) {
        }
    }
}