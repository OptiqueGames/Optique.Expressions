using System.Text;

namespace Optique.Expressions
{
    public class Expression : IValueGetter
    {
        private readonly IValueGetter[] _arguments;
        private readonly BinaryOperator[] _operators;

        internal bool HasBrackets { get; set; }


        internal Expression(BinaryOperator[] operators, IValueGetter[] arguments)
        {
            _arguments = arguments;
            _operators = operators;
            HasBrackets = false;
        }

        dynamic IValueGetter.GetValue()
        {
            return Calculate();
        }

        public dynamic Calculate()
        {
            if (_arguments.Length == 0)
            {
                return 0;
            }

            int operatorsCount = _operators.Length;

            if (operatorsCount >= _arguments.Length)
            {
                operatorsCount -= (operatorsCount - _arguments.Length) + 1;
            }

            if (_operators.Length == 1 && _operators[0].IsMatch(Operators.Assignment))
            {
                return _operators[0].Operate(_arguments[0], _arguments[1]);
            }
            else
            {
                Literal result = new Literal(_arguments[0].GetValue());

                for (int i = 0; i < operatorsCount; ++i)
                {
                    result = new Literal(_operators[i].Operate(result, _arguments[i + 1]));
                }

                return result.GetValue();
            }
        }

        public override string ToString()
        {
            if (_arguments == null || _arguments.Length == 0)
            {
                return string.Empty;
            }

            StringBuilder result = new StringBuilder();
            if (HasBrackets)
            {
                result.Append('(');
            }

            for (int i = 0; i < _arguments.Length; ++i)
            {
                result.Append(_arguments[i]);

                if (i < _operators.Length)
                {
                    result.Append($" {_operators[i]} ");
                }
            }

            if (HasBrackets)
            {
                result.Append(')');
            }

            return result.ToString();
        }
    }
}