namespace Frigg {
    public class EnableIfAttribute : ValidatorAttribute {
        public EnableIfAttribute(string name) : base(name) {
        }

        public EnableIfAttribute(string name, object expected) : base(name, expected) {
        }
    }
}