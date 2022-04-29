using System;
using System.Linq;
using System.Reflection;

namespace Optique.Expressions
{
    public class ExpressionEvaluator
    {
        public BinaryOperatorParserSettings BinaryOperatorParsingSettings { get; } = new BinaryOperatorParserSettings();
        public LiteralParserSettings LiteralParsingSettings { get; } = new LiteralParserSettings();
        public VariableParserSettings VariableParsingSettings { get; } = new VariableParserSettings();
        public FunctionParserSettings FunctionParserSettings { get; } = new FunctionParserSettings();

        private readonly BinaryOperatorParser _binaryOperatorParser;
        private readonly LiteralParser _literalParser;
        private readonly VariableParser _variableParser;
        private readonly ExpressionParser _expressionParser;
        private readonly FunctionParser _functionParser;


        private class ParserWrapper<T> : IParser<T>
        {
            private IParser<T> _parser;
            internal void Initialize(IParser<T> parser) => _parser = parser;
            bool IParser<T>.Validate(string unparsedValue) => _parser.Validate(unparsedValue);
            T IParser<T>.Parse(string unparsedValue) => _parser.Parse(unparsedValue);
        }

        public ExpressionEvaluator()
        {
            ParserWrapper<Expression> expressionParserWrapper = new ParserWrapper<Expression>();

            _binaryOperatorParser = new BinaryOperatorParser(BinaryOperatorParsingSettings);
            _literalParser = new LiteralParser(LiteralParsingSettings);
            _variableParser = new VariableParser(VariableParsingSettings);
            _functionParser = new FunctionParser(FunctionParserSettings, expressionParserWrapper);
            _expressionParser =
                    new ExpressionParser(_literalParser, _variableParser, _functionParser, _binaryOperatorParser);

            expressionParserWrapper.Initialize(_expressionParser);
        }

        public ExpressionParser GetExpressionParser()
        {
            return _expressionParser;
        }

        public void RegisterFunctionsFromType(Type containerType)
        {
            RegisterFunctions(containerType.GetMethods(BindingFlags.Static | BindingFlags.Public));
        }

        public void RegisterFunctions(params MethodInfo[] methods)
        {
            foreach (MethodInfo methodInfo in methods)
            {
                if (methodInfo.IsStatic == false)
                {
                    continue;
                }

                FunctionParserSettings.AddFunction(methodInfo);
            }
        }

        public void RegisterConstantsFromType(Type containerType)
        {
            FieldInfo[] fields = containerType
                    .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                    .ToArray();

            foreach (FieldInfo fieldInfo in fields)
            {
                RegisterVariable(fieldInfo.Name, () => fieldInfo.GetValue(null));
            }
        }

        public void RegisterVariable(string name, Func<dynamic> getter)
        {
            VariableParsingSettings.AddVariable(new ReadOnlyVariable(getter, () => name));
        }

        public void RegisterVariable(string name, Func<dynamic> getter, Action<dynamic> setter)
        {
            VariableParsingSettings.AddVariable(new Variable(getter, setter, () => name));
        }

        public void RegisterVariable(IReadOnlyValueField variable)
        {
            VariableParsingSettings.AddVariable(variable);
        }
        
        public bool IsVariableRegistered(string name)
        {
            return VariableParsingSettings.ContainsVariable(name);
        }

        public bool IsVariableRegistered(IReadOnlyValueField variable)
        {
            return VariableParsingSettings.ContainsVariable(variable);
        }

        public void RemoveVariable(IReadOnlyValueField variable)
        {
            VariableParsingSettings.RemoveVariable(variable);
        }

        public void RemoveVariable(string variableName)
        {
            VariableParsingSettings.RemoveVariable(variableName);
        }
    }
}