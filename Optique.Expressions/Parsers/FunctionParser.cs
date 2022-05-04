using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Optique.Reflection;

namespace Optique.Expressions
{
	internal class FunctionParser : IParser<Function>
	{
		private readonly IParser<IValueGetter> _expressionParser;
		private readonly FunctionParserSettings _settings;


		public FunctionParser(FunctionParserSettings settings, IParser<IValueGetter> expressionParser)
		{
			_settings = settings;
			_expressionParser = expressionParser;
		}

		public bool Validate(string function)
		{
			if (_settings.IsActive == false || string.IsNullOrEmpty(function))
			{
				return false;
			}

			int openBracketIndex = function.IndexOf('(');
			bool hasFunctionName = openBracketIndex > 0;
			bool lastCharIsCloseBracket = function[function.Length - 1] == ')';

			for (int i = 0; i < openBracketIndex; ++i)
			{
				char character = function[i];
				if (char.IsLetterOrDigit(character) == false && character != '_')
				{
					return false;
				}
			}

			if (hasFunctionName && lastCharIsCloseBracket)
			{
				string parameters = ParsingUtility.GetBracketsExpression(function, openBracketIndex);

				if (openBracketIndex + parameters.Length + 2 == function.Length)
				{
					return true;
				}
			}

			return false;
		}

		public Function Parse(string unparsedValue)
		{
			FunctionName name = GetFunctionName(unparsedValue);

			string[] unparsedParameters = ParsingUtility.SplitParameters(unparsedValue.Substring(name.FullName.Length));
			IValueGetter[] arguments = unparsedParameters.Select(_expressionParser.Parse).ToArray();

			return new Function(GetMethod(name.MethodName, GetTypes(arguments)), arguments);
		}


		private Type[] GetTypes(IValueGetter[] valueProviders)
		{
			return valueProviders.Select(provider =>
			{
				Type type = provider.GetValue().GetType();
				return type;
			}).ToArray();
		}

		private MethodInfo GetMethod(string name, Type[] argumentsTypes)
		{
			List<MethodInfo> methods = new List<MethodInfo>(_settings.GetFunctions(name));
			int argumentsCount = argumentsTypes.Length;
			List<MethodInfo> applicableMethods = new List<MethodInfo>();

			foreach (var method in methods)
			{
				Type[] methodArgumentsTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

				if (methodArgumentsTypes.Length == argumentsCount)
				{
					bool isAbsolutelyMatch = argumentsTypes
							.Zip(methodArgumentsTypes, (left, right) => left == right)
							.All(match => match);
					
					if (isAbsolutelyMatch)
					{
						return method;
					}
					
					bool successfullyMatched = true;
					for (int i = 0; i < argumentsCount; ++i)
					{
						if (argumentsTypes[i].IsCastableTo(methodArgumentsTypes[i]) == false)
						{
							successfullyMatched = false;
							break;
						}
					}

					if (successfullyMatched)
					{
						applicableMethods.Add(method);
					}
				}
			}

			if(applicableMethods.Count == 0)
			{
				StringBuilder errorMessage = new StringBuilder($"Method {name}(");
				for (int i = 0; i < argumentsTypes.Length; ++i)
				{
					errorMessage.Append(argumentsTypes[i]);

					if (i < argumentsTypes.Length - 1)
					{
						errorMessage.Append(", ");
					}
				}

				errorMessage.Append(") does not found");

				throw new Exception(errorMessage.ToString());
			}
			else
			{
				//TODO: check if all arguments are castable
				return applicableMethods[0];
			}
		}

		private static FunctionName GetFunctionName(string sourceText)
		{
			string name = sourceText.Substring(0, sourceText.IndexOf('('));
			FunctionName functionName = new FunctionName();

			string[] names = name.Split('.');
			if (names.Length > 3)
			{
				throw new Exception("Inner types and/or namespace parsing is not supported");
			}

			int namesCount = names.Length;

			functionName.MethodName = names[namesCount - 1];
			if (namesCount > 1)
			{
				functionName.TypeName = names[namesCount - 2];
			}

			return functionName;
		}

		private struct FunctionName
		{
			public string Namespace;
			public string TypeName;
			public string MethodName;

			public string FullName
			{
				get
				{
					StringBuilder name = new StringBuilder();

					if (string.IsNullOrEmpty(TypeName) == false)
					{
						if (string.IsNullOrEmpty(Namespace) == false)
						{
							name.Append(Namespace);
							name.Append('.');
						}

						name.Append(TypeName);
						name.Append('.');
					}

					name.Append(MethodName);

					return name.ToString();
				}
			}
		}
	}
}