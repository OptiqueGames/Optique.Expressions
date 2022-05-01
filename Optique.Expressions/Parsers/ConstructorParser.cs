using System;
using System.Linq;

namespace Optique.Expressions
{
    public class ConstructorParser : IParser<Constructor>
    {
        private const string NewOperatorSubstring = "new ";
        
        private readonly IParser<IValueGetter> _expressionParser;
        private readonly ConstructorParserSettings _settings;
        
        
        public ConstructorParser(ConstructorParserSettings settings, IParser<IValueGetter> expressionParser)
        {
            _expressionParser = expressionParser;
            _settings = settings;
        }
        
        public bool Validate(string unparsedValue)
        {
            return _settings.IsActive && unparsedValue.StartsWith("new") &&
                   HasOnlyConstructorCall(unparsedValue) &&
                   _settings.ContainsType(GetTypeName(unparsedValue));
        }
        
        public Constructor Parse(string unparsedValue)
        {
            string typeName = GetTypeName(unparsedValue);
            string[] unparsedParameters = ParsingUtility.SplitParameters(unparsedValue.Substring(unparsedValue.IndexOf('(')));
            IValueGetter[] arguments = unparsedParameters.Select(_expressionParser.Parse).ToArray();
            Type constructorType = _settings.GetType(typeName);
            
            return new Constructor(constructorType, arguments);
        }

        private bool HasOnlyConstructorCall(string unparsedValue)
        {
            int bracketsCount = 0;
            
            for (int i = 0; i < unparsedValue.Length; ++i)
            {
                if (unparsedValue[i] == '(')
                {
                    ++bracketsCount;
                }
                else if (unparsedValue[i] == ')')
                {
                    --bracketsCount;

                    if (bracketsCount == 0)
                    {
                        return i == unparsedValue.Length - 1;
                    }
                }
            }

            return false;
        }
        
        private string GetTypeName(string sourceText)
        {
            string result = sourceText.Substring(0, sourceText.IndexOf('('));
            result = result.Remove(0, NewOperatorSubstring.Length);
            return result;
        }
    }
}