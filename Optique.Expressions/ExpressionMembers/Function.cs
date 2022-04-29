using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Optique.Expressions
{
	public class Function : IValueGetter
	{
		private readonly IValueGetter[] _arguments;
		private readonly MethodInfo _methodInfo;
		private readonly string _name;


		public Function(MethodInfo methodInfo, IValueGetter[] arguments)
		{
			_name = methodInfo.Name;
			_methodInfo = methodInfo;
			_arguments = arguments;
		}

		dynamic IValueGetter.GetValue()
		{
			return Calculate();
		}

		private dynamic Calculate()
		{
			MethodInfo meth = new Func<dynamic>(Calculate).Method;
			return _methodInfo.Invoke(null, _arguments.Select(arg => (object) arg.GetValue()).ToArray());
		}

		public override string ToString()
		{
			StringBuilder result = new StringBuilder(_name);
			result.Append('(');

			for (int i = 0; i < _arguments.Length; ++i)
			{
				result.Append(_arguments[i]);

				if (i < _arguments.Length - 1)
				{
					result.Append(", ");
				}
			}

			result.Append(')');

			return result.ToString();
		}
	}
}