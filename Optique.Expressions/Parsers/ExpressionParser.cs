using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Optique.Expressions
{
    public class ExpressionParser : IParser<Expression>
    {
        private readonly IParser<Literal> _literalParser;
        private readonly IParser<IReadOnlyValueField> _variableParser;
        private readonly IParser<Function> _functionParser;
        private readonly IParser<Constructor> _constructorParser;
        private readonly IParser<BinaryOperator> _operatorParser;
        private readonly IParser<IValueGetter>[] _parsers;
        private readonly IExpressionEvaluator _expressionEvaluator;

        internal ExpressionParser(IExpressionEvaluator expressionEvaluator,
                IParser<Literal> literalParser,
                IParser<IReadOnlyValueField> variableParser,
                IParser<Function> functionParser,
                IParser<Constructor> constructorParser,
                IParser<BinaryOperator> operatorParser)
        {
            _expressionEvaluator = expressionEvaluator;
            
            _literalParser = literalParser;
            _variableParser = variableParser;
            _functionParser = functionParser;
            _constructorParser = constructorParser;
            _operatorParser = operatorParser;

            _parsers = new IParser<IValueGetter>[] {_literalParser, _variableParser, _functionParser, _constructorParser};
        }

        public bool Validate(string unparsedValue)
        {
            //TODO: implement full validation
            if (string.IsNullOrEmpty(unparsedValue) || string.IsNullOrWhiteSpace(unparsedValue))
            {
                return false;
            }

            string[] identifiers = ExtractIdentifiers(unparsedValue);

            foreach (string identifier in identifiers)
            {
                if ((_expressionEvaluator.VariableParsingSettings.IsActive && _expressionEvaluator.IsVariableRegistered(identifier)) ||
                    (_expressionEvaluator.FunctionParserSettings.IsActive && _expressionEvaluator.IsFunctionRegistered(identifier)) ||
                    (_expressionEvaluator.ConstructorParserSettings.IsActive && _expressionEvaluator.IsConstructorRegistered(identifier)))
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private string[] ExtractIdentifiers(string source)
        {
            StringBuilder word = new StringBuilder();
            List<string> result = new List<string>();

            foreach (char c in source)
            {
                if (word.Length == 0)
                {
                    if (char.IsLetter(c) || c == '_')
                    {
                        word.Append(c);
                    }
                }
                else
                {
                    if (char.IsLetterOrDigit(c) || c == '_')
                    {
                        word.Append(c);
                    }
                    else
                    {
                        result.Add(word.ToString());
                        word.Clear();
                    }
                }
            }

            if (word.Length > 0)
            {
                result.Add(word.ToString());
                word.Clear();
            }

            return result.ToArray();
        }

        private IValueGetter BuildArgument(IValueGetter valueGetter, bool hasUnaryMinus, bool hasUnaryNegation)
        {
            if (hasUnaryMinus || hasUnaryNegation)
            {
                return new ExpressionArgument(valueGetter, hasUnaryMinus, hasUnaryNegation);
            }
            else
            {
                return valueGetter;
            }
        }

        private bool ValidateWithParsers(string unparsedValue)
        {
            return _parsers.Any(parser => parser.Validate(unparsedValue));
        }

        private IValueGetter ParseWithParsers(string unparsedValue, bool includingExpressionParser = false)
        {
            foreach (var parser in _parsers)
            {
                if (parser.Validate(unparsedValue))
                {
                    return parser.Parse(unparsedValue);
                }
            }

            return includingExpressionParser ? Parse(unparsedValue) : null;
        }

        public Expression Parse(string expression)
        {
            expression = ParsingUtility.ClearWhiteSpaces(expression);

            List<IValueGetter> arguments = new List<IValueGetter>();
            List<BinaryOperator> operators = new List<BinaryOperator>();

            bool nextOperandHasUnaryNegation = false;
            bool nextOperandHasUnaryMinus = false;

            if (Validate(expression) == false)
            {
                throw new Exception();
            }

            StringBuilder operand = new StringBuilder();

            for (int i = 0; i < expression.Length; ++i)
            {
                if (ParsingUtility.IsOperator(expression[i]))
                {
                    if (i + 1 >= expression.Length)
                    {
                        throw new Exception();
                    }

                    string operatorString = expression[i].ToString();

                    if (ParsingUtility.IsOperator(expression[i + 1]))
                    {
                        operatorString += expression[i + 1];
                        ++i;
                    }

                    if (_operatorParser.Validate(operatorString) == false)
                    {
                        if (operatorString.Equals("!"))
                        {
                            nextOperandHasUnaryNegation = true;
                            continue;
                        }

                        throw new Exception();
                    }

                    BinaryOperator parsedOperator = _operatorParser.Parse(operatorString);
                    operators.Add(parsedOperator);

                    if (operand.Length > 0)
                    {
                        IValueGetter nextArgument = ParseWithParsers(operand.ToString(), true);
                        arguments.Add(
                            BuildArgument(nextArgument, nextOperandHasUnaryMinus, nextOperandHasUnaryNegation));
                        nextOperandHasUnaryMinus = nextOperandHasUnaryNegation = false;
                        operand.Clear();
                    }
                    else if (arguments.Count == 0 || operators.Count > arguments.Count)
                    {

                        if (parsedOperator.IsMatch(Operators.Subtraction))
                        {
                            nextOperandHasUnaryMinus = true;
                            operators.RemoveAt(operators.Count - 1);
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                }
                else if (expression[i] == '(')
                {
                    string bracketsExpression = ParsingUtility.GetBracketsExpression(expression, i);
                    bool isFunction = operand.Length > 0;

                    if (isFunction)
                    {
                        operand.Append('(').Append(bracketsExpression).Append(')');
                    }
                    else
                    {
                        operand.Append(bracketsExpression);
                    }

                    IValueGetter arg = ParseWithParsers(operand.ToString(), true);

                    if (arg is Expression ex)
                    {
                        ex.HasBrackets = true;
                        arg = ex;
                    }

                    arguments.Add(BuildArgument(arg, nextOperandHasUnaryMinus, nextOperandHasUnaryNegation));
                    nextOperandHasUnaryMinus = nextOperandHasUnaryNegation = false;

                    operand.Clear();
                    i += bracketsExpression.Length + 1;
                }
                else
                {
                    operand.Append(expression[i]);
                }
            }

            if (operand.Length > 0)
            {
                string operandString = operand.ToString();
                if (ValidateWithParsers(operandString) == false)
                {
                    throw new Exception($"Invalid operand {operandString}");
                }

                IValueGetter value = ParseWithParsers(operandString);
                if (value != null && value is Expression == false)
                {
                    arguments.Add(BuildArgument(value, nextOperandHasUnaryMinus, nextOperandHasUnaryNegation));
                }
            }

            for (int i = 0; i < operators.Count; ++i)
            {
                if (operators[i].IsMatch(Operators.Multiplication) || operators[i].IsMatch(Operators.Division))
                {
                    Expression ex = new Expression
                    (
                        new[] {operators[i]},
                        new[] {arguments[i], arguments[i + 1]}
                    );

                    arguments[i] = BuildArgument(ex, false, false);
                    operators.RemoveAt(i);
                    arguments.RemoveAt(i + 1);
                }
            }

            int assignmentsCount = operators.Count(op => op.IsMatch(Operators.Assignment));
            if (assignmentsCount > 0 && operators.Count > 1)
            {
                for (int i = 0; i < assignmentsCount; ++i)
                {
                    if (operators[i].IsMatch(Operators.Assignment) == false)
                    {
                        //TODO: add error description
                        throw new Exception();
                    }
                }

                if (operators.Count > assignmentsCount)
                {
                    Expression right = new Expression(
                            operators.GetRange(assignmentsCount, operators.Count - assignmentsCount).ToArray(),
                            arguments.GetRange(assignmentsCount, arguments.Count - assignmentsCount).ToArray());

                    operators.RemoveRange(assignmentsCount, operators.Count - assignmentsCount);
                    arguments.RemoveRange(assignmentsCount, arguments.Count - assignmentsCount);

                    arguments.Add(BuildArgument(right, false, false));
                }

                if (assignmentsCount > 1)
                {
                    while (operators.Count > 1)
                    {
                        Expression temp = new Expression(new[] {operators[operators.Count - 1]},
                                new[] {arguments[arguments.Count - 2], arguments[arguments.Count - 1]});

                        operators.RemoveAt(operators.Count - 1);
                        arguments.RemoveAt(arguments.Count - 1);
                        arguments[arguments.Count - 1] = BuildArgument(temp, false, false);
                    }
                }
            }

            return new Expression(operators.ToArray(), arguments.ToArray());
        }
    }
}