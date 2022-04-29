using System;

namespace Optique.Expressions
{
	public readonly struct BinaryOperator
	{
		private readonly string _operatorSymbol;
		private readonly Func<IValueGetter, IValueGetter, dynamic> _operation;


		internal BinaryOperator(string operatorSymbol, Func<IValueGetter, IValueGetter, dynamic> operation)
		{
			_operatorSymbol = operatorSymbol;
			_operation = operation;
		}

		internal dynamic Operate(IValueGetter leftOperand, IValueGetter rightOperand)
		{
			return _operation(leftOperand, rightOperand);
		}

		public override string ToString()
		{
			return _operatorSymbol;
		}
	}
}