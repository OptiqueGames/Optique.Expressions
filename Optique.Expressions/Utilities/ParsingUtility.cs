using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Optique.Expressions
{
    internal static class ParsingUtility
    {
        private const string NewOperatorSubstring = "new ";
        private const string TemporaryUtilitySubstring = "[~]";


        public static string ClearWhiteSpaces(string unparsedValue)
        {
            if (unparsedValue.Contains(NewOperatorSubstring))
            {
                unparsedValue = unparsedValue.Replace(NewOperatorSubstring, TemporaryUtilitySubstring);
                unparsedValue = Regex.Replace(unparsedValue, @"\s+", "");
                return unparsedValue.Replace(TemporaryUtilitySubstring, NewOperatorSubstring);
            }
            else
            {
                return Regex.Replace(unparsedValue, @"\s+", "");
            }
        }

        public static bool IsOperator(char symbol)
        {
            return symbol == '+' ||
                   symbol == '-' ||
                   symbol == '/' ||
                   symbol == '*' ||
                   symbol == '%' ||
                   symbol == '=' ||
                   symbol == '!' ||
                   symbol == '&' ||
                   symbol == '|' ||
                   symbol == '>' ||
                   symbol == '<';
        }

        public static string GetBracketsExpression(string source, int openBracketIndex)
        {
            StringBuilder result = new StringBuilder();

            int innerBrackets = 0;
            for (int i = openBracketIndex + 1; i < source.Length; ++i)
            {
                if (source[i] == '(')
                {
                    ++innerBrackets;
                }
                else if (source[i] == ')' && --innerBrackets < 0)
                {
                    break;
                }

                result.Append(source[i]);
            }

            return result.ToString();
        }

        public static string[] SplitParameters(string source)
        {
            source = source.Substring(1, source.Length - 2);

            List<string> parametersList = new List<string>();

            StringBuilder parameter = new StringBuilder();

            int innerBrackets = 0;
            for (int i = 0; i < source.Length; ++i)
            {
                if (source[i] == '(')
                {
                    ++innerBrackets;
                }
                else if (source[i] == ')')
                {
                    --innerBrackets;
                }
                else if (source[i] == ',' && innerBrackets == 0)
                {
                    parametersList.Add(parameter.ToString());
                    parameter.Clear();
                    continue;
                }

                parameter.Append(source[i]);
            }

            if (parameter.Length > 0)
            {
                parametersList.Add(parameter.ToString());
            }

            return parametersList.ToArray();
        }
    }
}