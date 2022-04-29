namespace Optique.Expressions
{
    public static class BinaryOperatorParserSettingsExtensions
    {
        public static void SetParsingArithmeticOperators(this BinaryOperatorParserSettings settings, bool isParse)
        {
            settings.ParseAddition =
                    settings.ParseSubtraction =
                            settings.ParseMultiplication =
                                    settings.ParseDivision =
                                            settings.ParseRemainder = isParse;
        }

        public static void SetParsingComparisonOperators(this BinaryOperatorParserSettings settings, bool isParse)
        {
            settings.ParseLogicalAND =
                    settings.ParseLogicalOR =
                            settings.ParseConditionalLogicalAND =
                                    settings.ParseConditionalLogicalOR = isParse;
        }

        public static void SetParsingBooleanLogicalOperators(this BinaryOperatorParserSettings settings, bool isParse)
        {
            settings.ParseLessThan =
                    settings.ParseGreaterThan =
                            settings.ParseLessThanOrEqual =
                                    settings.ParseGreaterThanOrEqual = isParse;
        }

        public static void SetParsingEqualityOperators(this BinaryOperatorParserSettings settings, bool isParse)
        {
            settings.ParseEquality =
                    settings.ParseInequality = isParse;
        }
    }
}