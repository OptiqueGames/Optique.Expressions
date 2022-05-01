namespace Optique.Expressions
{
    public class BinaryOperatorParserSettings : ParserSettings
    {
        public bool ParseAddition = true;
        public bool ParseSubtraction = true;
        public bool ParseMultiplication = true;
        public bool ParseDivision = true;
        public bool ParseRemainder = true;

        public bool ParseLogicalAND = true;
        public bool ParseLogicalOR = true;
        public bool ParseConditionalLogicalAND = true;
        public bool ParseConditionalLogicalOR = true;

        public bool ParseEquality = true;
        public bool ParseInequality = true;

        public bool ParseLessThan = true;
        public bool ParseGreaterThan = true;
        public bool ParseLessThanOrEqual = true;
        public bool ParseGreaterThanOrEqual = true;
    }
}