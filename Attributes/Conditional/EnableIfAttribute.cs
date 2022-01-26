namespace Frigg {
    public class EnableIfAttribute : ConditionAttribute {
        public EnableIfAttribute(string expression) : base(expression) {
        }

        public EnableIfAttribute(string name, bool expected) : base(name, expected) {
        }
    }
}