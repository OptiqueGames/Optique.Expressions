using System;
using System.Text.RegularExpressions;

namespace Optique.Expressions
{
    internal class LiteralParser : IParser<Literal>
    {
        private readonly LiteralParserSettings _settings;


        public LiteralParser(LiteralParserSettings settings)
        {
            _settings = settings;
        }

        public bool Validate(string unparsedValue)
        {
            unparsedValue = RemoveSuffix(unparsedValue);

            return _settings.ParseInt && int.TryParse(unparsedValue, out int _) ||
                   _settings.ParseFloat && float.TryParse(unparsedValue, out float _) ||
                   _settings.ParseBool && bool.TryParse(unparsedValue, out bool _) ||
                   _settings.ParseString && IsString(unparsedValue);
        }

        public Literal Parse(string unparsedValue)
        {
            if (Validate(unparsedValue) == false)
            {
                throw new Exception($"The literal \"{unparsedValue}\" is not recognized");
            }

            dynamic value;

            unparsedValue = RemoveSuffix(unparsedValue);

            if (int.TryParse(unparsedValue, out int intNumber))
            {
                value = intNumber;
            }
            else if (float.TryParse(unparsedValue, out float floatNumber))
            {
                value = floatNumber;
            }
            else if (bool.TryParse(unparsedValue, out bool booleanValue))
            {
                value = booleanValue;
            }
            else if (IsString(unparsedValue))
            {
                value = new string(unparsedValue.ToCharArray(1, unparsedValue.Length - 2));
            }
            else
            {
                throw new Exception($"The literal \"{unparsedValue}\" is not recognized");
            }

            return new Literal(value);
        }

        private string RemoveSuffix(string unparsedValue)
        {
            if (char.IsDigit(unparsedValue[0]))
            {
                unparsedValue = unparsedValue.ToLower();
                char lastChar = unparsedValue[unparsedValue.Length - 1];
                if (unparsedValue.EndsWith("ul") || unparsedValue.EndsWith("lu"))
                {
                    unparsedValue = new string(unparsedValue.ToCharArray(0, unparsedValue.Length - 2));
                }
                else if (lastChar == 'f' || lastChar == 'd' || lastChar == 'm' || lastChar == 'u' || lastChar == 'l')
                {
                    unparsedValue = new string(unparsedValue.ToCharArray(0, unparsedValue.Length - 1));
                }
            }

            return unparsedValue;
        }

        private bool IsString(string unparsedValue)
        {
            return unparsedValue.StartsWith("\"") && unparsedValue.EndsWith("\"") &&
                   Regex.Matches(unparsedValue, "\"").Count == 2;
        }
    }
}