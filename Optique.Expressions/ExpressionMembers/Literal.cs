namespace Optique.Expressions
{
	public class Literal : IValueGetter
	{
		private readonly dynamic _value;


		public Literal(dynamic value)
		{
			_value = value;
		}

		public dynamic GetValue()
		{
			return _value;
		}

		public override string ToString()
		{
			if (_value is bool booleanValue)
			{
				return booleanValue.ToString().ToLower();
			}
			else if (_value is string stringValue)
			{
				return $"\"{stringValue}\"";
			}
			else
			{
				return _value.ToString();
			}
		}
	}
}