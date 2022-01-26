namespace Frigg {
    public class EnableIfAttribute : ConditionAttribute {
        public EnableIfAttribute(string name) : base(name) {
        }

        public EnableIfAttribute(string name, object expected) : base(name, expected) {
        }
    }
}