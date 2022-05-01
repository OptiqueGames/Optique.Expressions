using System;

namespace Optique.Expressions
{
    public readonly struct ExpressionArgument : IValueGetter
    {
        private readonly IValueGetter _valueGetter;

        private readonly bool _hasUnaryMinus;
        private readonly bool _hasUnaryNegation;


        public ExpressionArgument(IValueGetter valueGetter, bool hasUnaryMinus = false, bool hasUnaryNegation = false)
        {
            _valueGetter = valueGetter;

            _hasUnaryMinus = hasUnaryMinus;
            _hasUnaryNegation = hasUnaryNegation;

            if (hasUnaryMinus && hasUnaryNegation)
            {
                throw new Exception();
            }
        }

        public dynamic GetValue()
        {
            if (_hasUnaryNegation)
            {
                return !_valueGetter.GetValue();
            }
            else if (_hasUnaryMinus)
            {
                return -_valueGetter.GetValue();
            }

            return _valueGetter.GetValue();
        }

        public override string ToString()
        {
            string prefix = string.Empty;

            if (_hasUnaryNegation)
            {
                prefix = "!";
            }
            else if (_hasUnaryMinus)
            {
                prefix = "-";
            }

            return $"{prefix}{_valueGetter}";
        }
    }
}