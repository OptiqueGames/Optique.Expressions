using System;
using System.Collections.Generic;

namespace Optique.Expressions
{
    internal class BinaryOperatorParser : IParser<BinaryOperator>
    {
        private static readonly Dictionary<string, Func<IValueGetter, IValueGetter, dynamic>> Operations =
                new Dictionary<string, Func<IValueGetter, IValueGetter, dynamic>>
                {
                        {Operators.Addition, (left, right) => left.GetValue() + right.GetValue()},
                        {Operators.Subtraction, (left, right) => left.GetValue() - right.GetValue()},
                        {Operators.Multiplication, (left, right) => left.GetValue() * right.GetValue()},
                        {Operators.Division, (left, right) => left.GetValue() / right.GetValue()},
                        {Operators.Remainder, (left, right) => left.GetValue() % right.GetValue()},

                        {Operators.Equality, (left, right) => left.GetValue() == right.GetValue()},
                        {Operators.Inequality, (left, right) => left.GetValue() != right.GetValue()},

                        {Operators.LessThan, (left, right) => left.GetValue() < right.GetValue()},
                        {Operators.GreaterThan, (left, right) => left.GetValue() > right.GetValue()},
                        {Operators.LessThanOrEqual, (left, right) => left.GetValue() <= right.GetValue()},
                        {Operators.GreaterThanOrEqual, (left, right) => left.GetValue() >= right.GetValue()},

                        {Operators.LogicalAND, (left, right) => left.GetValue() & right.GetValue()},
                        {Operators.LogicalOR, (left, right) => left.GetValue() | right.GetValue()},
                        {Operators.ConditionalLogicalAND, (left, right) => left.GetValue() && right.GetValue()},
                        {Operators.ConditionalLogicalOR, (left, right) => left.GetValue() || right.GetValue()}
                };

        private readonly BinaryOperatorParserSettings _settings;


        internal BinaryOperatorParser(BinaryOperatorParserSettings settings)
        {
            _settings = settings;
        }

        public bool Validate(string unparsedValue)
        {
            if (_settings.IsActive == false || Operations.ContainsKey(unparsedValue) == false)
            {
                return false;
            }

            switch (unparsedValue)
            {
                case Operators.Addition: return _settings.ParseAddition;
                case Operators.Subtraction: return _settings.ParseSubtraction;
                case Operators.Multiplication: return _settings.ParseMultiplication;
                case Operators.Division: return _settings.ParseDivision;
                case Operators.Remainder: return _settings.ParseRemainder;

                case Operators.Equality: return _settings.ParseEquality;
                case Operators.Inequality: return _settings.ParseInequality;

                case Operators.LessThan: return _settings.ParseLessThan;
                case Operators.GreaterThan: return _settings.ParseGreaterThan;
                case Operators.LessThanOrEqual: return _settings.ParseLessThanOrEqual;
                case Operators.GreaterThanOrEqual: return _settings.ParseGreaterThanOrEqual;

                case Operators.LogicalAND: return _settings.ParseLogicalAND;
                case Operators.LogicalOR: return _settings.ParseLogicalOR;
                case Operators.ConditionalLogicalAND: return _settings.ParseConditionalLogicalAND;
                case Operators.ConditionalLogicalOR: return _settings.ParseConditionalLogicalOR;

                default: return false;
            }
        }

        public BinaryOperator Parse(string unparsedValue)
        {
            if (Validate(unparsedValue))
            {
                return new BinaryOperator(unparsedValue, Operations[unparsedValue]);
            }
            else
            {
                throw new Exception("Invalid operator signature");
            }
        }
    }
}