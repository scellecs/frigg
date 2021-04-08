namespace Frigg {
    public class DisableIfAttribute : ValidatorAttribute {
        public DisableIfAttribute(string name) : base(name) {
        }

        public DisableIfAttribute(string name, object expected) : base(name, expected) {
        }
    }
}