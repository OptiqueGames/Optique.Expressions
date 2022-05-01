namespace Optique.Expressions
{
    public sealed class LiteralParserSettings : ParserSettings
    {
        public bool ParseInt = true;
        public bool ParseFloat = true;
        public bool ParseBool = true;
        public bool ParseString = true;
    }
}