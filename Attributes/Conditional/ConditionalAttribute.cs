namespace Frigg {
    using System;

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method,
        Inherited = false, AllowMultiple = true)]
    public abstract class ConditionAttribute : Attribute, IAttribute {
        public EConditionType ConditionType { get; }
        public string         MemberName    { get; }
        public string         Expression    { get; }

        public bool ExpectedValue { get; private set; }

        protected ConditionAttribute(string expression) {
            this.Expression    = expression;
            this.ConditionType = EConditionType.ByExpression;
        }

        protected ConditionAttribute(string memberName, bool expectedValue) {
            this.ConditionType = EConditionType.ByCondition;
            this.MemberName    = memberName;
            this.ExpectedValue = expectedValue;
        }
    }

    public enum EConditionType {
        ByExpression = 0,
        ByCondition = 1 
    }
}