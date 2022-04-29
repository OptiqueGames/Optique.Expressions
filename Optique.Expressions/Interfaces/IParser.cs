namespace Optique.Expressions
{
    public interface IParser<out T>
    {
        bool Validate(string unparsedValue);
        T Parse(string unparsedValue);
    }
}