﻿namespace Erdcsharp.Domain.Values
{
    public class OptionValue : BaseBinaryValue
    {
        public TypeValue InnerType { get; }
        public IBinaryType Value { get; }

        private OptionValue(TypeValue type, TypeValue innerType = null, IBinaryType value = null) : base(type)
        {
            InnerType = innerType;
            Value = value;
        }

        public static OptionValue NewMissing()
        {
            return new OptionValue(TypeValue.OptionValue());
        }

        public static OptionValue NewProvided(IBinaryType value)
        {
            return new OptionValue(TypeValue.OptionValue(value.Type), value.Type, value);
        }

        public bool IsSet()
        {
            return Value != null;
        }

        public override string ToString()
        {
            return IsSet() ? Value.ToString() : "";
        }

        new string ToJSON()
        {
            return IsSet() ? Value.ToJSON() : "{}";
        }
    }
}