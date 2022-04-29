namespace Optique.Expressions
{
    internal static class BinaryOperatorExtensions
    {
        public static bool IsMatch(this BinaryOperator binaryOperator, string text)
        {
            return binaryOperator.ToString().Equals(text);
        }
    }
}