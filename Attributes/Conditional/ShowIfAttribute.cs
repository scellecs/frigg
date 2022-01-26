namespace Frigg {
    public class ShowIfAttribute : ConditionAttribute {
        public ShowIfAttribute(string expression) : base(expression) {
        }

        public ShowIfAttribute(string name, bool expected) : base(name, expected) {
        }
    }
}