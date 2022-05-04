namespace Optique.Expressions
{
    internal interface IExpressionEvaluator
    {
        BinaryOperatorParserSettings BinaryOperatorParsingSettings { get; }
        LiteralParserSettings LiteralParsingSettings { get; }
        VariableParserSettings VariableParsingSettings { get; }
        FunctionParserSettings FunctionParserSettings { get; }
        ConstructorParserSettings ConstructorParserSettings { get; }
        
        bool IsConstructorRegistered(string name);
        bool IsFunctionRegistered(string name);
        bool IsVariableRegistered(string name);
    }
}