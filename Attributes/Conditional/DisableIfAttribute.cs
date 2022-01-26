namespace Frigg {
    public class DisableIfAttribute : ConditionAttribute {
        public DisableIfAttribute(string expression) : base(expression) {
        }

        public DisableIfAttribute(string name, bool expected) : base(name, expected) {
        }
    }
}