namespace Frigg {
    public class ShowIfAttribute : ConditionAttribute {
        public ShowIfAttribute(string name) : base(name) {
        }

        public ShowIfAttribute(string name, object expected) : base(name, expected) {
        }
    }
}